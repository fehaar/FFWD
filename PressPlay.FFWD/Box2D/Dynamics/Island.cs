/*
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Box2D.XNA
{
    /// This is an internal class.
    class Island
    {
        public Island() { }

        public void Reset(int bodyCapacity, int contactCapacity, int jointCapacity, IContactListener listener)
        {
            _bodyCapacity = bodyCapacity;
	        _contactCapacity = contactCapacity;
	        _jointCapacity	 = jointCapacity;
	        _bodyCount = 0;
	        _jointCount = 0;

	        _listener = listener;

            if (_bodies == null || _bodies.Length < bodyCapacity)
            {
                _bodies = new Body[bodyCapacity];
            }

            if (_contacts == null || _contacts.Length < contactCapacity)
            {
                _contacts = new Contact[contactCapacity * 2];
            }

            if (_joints == null || _joints.Length < jointCapacity)
            {
                _joints = new Joint[jointCapacity * 2];
            }
        }

	    public void Clear()
	    {
		    _bodyCount = 0;
	        _contactCount = 0;
		    _jointCount = 0;
	    }

	    public void Solve(ref TimeStep step, Vector2 gravity, bool allowSleep)
        {
            // Integrate velocities and apply damping.
	        for (int i = 0; i < _bodyCount; ++i)
	        {
		        Body b = _bodies[i];

                if (b.GetType() != BodyType.Dynamic)
                {
                    continue;
                }

		        // Integrate velocities.
		        b._linearVelocity += step.dt * (gravity + b._invMass * b._force);
		        b._angularVelocity += step.dt * b._invI * b._torque;

		        // Apply damping.
		        // ODE: dv/dt + c * v = 0
		        // Solution: v(t) = v0 * exp(-c * t)
		        // Time step: v(t + dt) = v0 * exp(-c * (t + dt)) = v0 * exp(-c * t) * exp(-c * dt) = v * exp(-c * dt)
		        // v2 = exp(-c * dt) * v1
		        // Taylor expansion:
		        // v2 = (1.0f - c * dt) * v1
		        b._linearVelocity *= MathUtils.Clamp(1.0f - step.dt * b._linearDamping, 0.0f, 1.0f);
		        b._angularVelocity *= MathUtils.Clamp(1.0f - step.dt * b._angularDamping, 0.0f, 1.0f);
	        }

            // Partition contacts so that contacts with static bodies are solved last.
	        int i1 = -1;
	        for (int i2 = 0; i2 < _contactCount; ++i2)
	        {
		        Fixture fixtureA = _contacts[i2].GetFixtureA();
		        Fixture fixtureB = _contacts[i2].GetFixtureB();
		        Body bodyA = fixtureA.GetBody();
		        Body bodyB = fixtureB.GetBody();
                bool nonStatic = bodyA.GetType() != BodyType.Static && bodyB.GetType() != BodyType.Static;
		        if (nonStatic)
		        {
			        ++i1;
			        //b2Swap(_contacts[i1], _contacts[i2]);
                    Contact temp = _contacts[i1];
                    _contacts[i1] = _contacts[i2];
                    _contacts[i2] = temp;
		        }
	        }

	        // Initialize velocity constraints.
            _contactSolver.Reset(_contacts, _contactCount, step.dtRatio);
	        _contactSolver.WarmStart();

	        for (int i = 0; i < _jointCount; ++i)
	        {
		        _joints[i].InitVelocityConstraints(ref step);
	        }

	        // Solve velocity constraints.
	        for (int i = 0; i < step.velocityIterations; ++i)
	        {
		        for (int j = 0; j < _jointCount; ++j)
		        {
			        _joints[j].SolveVelocityConstraints(ref step);
		        }

                _contactSolver.SolveVelocityConstraints();
	        }

	        // Post-solve (store impulses for warm starting).
            _contactSolver.StoreImpulses();

	        // Integrate positions.
	        for (int i = 0; i < _bodyCount; ++i)
	        {
		        Body b = _bodies[i];

                if (b.GetType() == BodyType.Static)
                {
                    continue;
                }

		        // Check for large velocities.
		        Vector2 translation = step.dt * b._linearVelocity;
		        if (Vector2.Dot(translation, translation) > Settings.b2_maxTranslationSquared)
		        {
			        float ratio = Settings.b2_maxTranslation / translation.Length();
                    b._linearVelocity *= ratio;
		        }

		        float rotation = step.dt * b._angularVelocity;
		        if (rotation * rotation > Settings.b2_maxRotationSquared)
		        {
                    float ratio = Settings.b2_maxRotation / Math.Abs(rotation);
                    b._angularVelocity *= ratio;
		        }

		        // Store positions for continuous collision.
		        b._sweep.c0 = b._sweep.c;
		        b._sweep.a0 = b._sweep.a;

		        // Integrate
		        b._sweep.c += step.dt * b._linearVelocity;
		        b._sweep.a += step.dt * b._angularVelocity;

		        // Compute new transform
		        b.SynchronizeTransform();

		        // Note: shapes are synchronized later.
	        }

	        // Iterate over constraints.
            for (int i = 0; i < step.positionIterations; ++i)
	        {
                bool contactsOkay = _contactSolver.SolvePositionConstraints(Settings.b2_contactBaumgarte);

		        bool jointsOkay = true;
		        for (int j = 0; j < _jointCount; ++j)
		        {
			        bool jointOkay = _joints[j].SolvePositionConstraints(Settings.b2_contactBaumgarte);
			        jointsOkay = jointsOkay && jointOkay;
		        }

		        if (contactsOkay && jointsOkay)
		        {
			        // Exit early if the position errors are small.
			        break;
		        }
	        }

            Report(_contactSolver._constraints);

	        if (allowSleep)
	        {
		        float minSleepTime = Settings.b2_maxFloat;

		        const float linTolSqr = Settings.b2_linearSleepTolerance * Settings.b2_linearSleepTolerance;
		        const float angTolSqr = Settings.b2_angularSleepTolerance * Settings.b2_angularSleepTolerance;

		        for (int i = 0; i < _bodyCount; ++i)
		        {
			        Body b = _bodies[i];
			        if (b.GetType() == BodyType.Static)
			        {
				        continue;
			        }

			        if ((b._flags & BodyFlags.AutoSleep) == 0)
			        {
				        b._sleepTime = 0.0f;
				        minSleepTime = 0.0f;
			        }

			        if ((b._flags & BodyFlags.AutoSleep) == 0 ||
				        b._angularVelocity * b._angularVelocity > angTolSqr ||
				        Vector2.Dot(b._linearVelocity, b._linearVelocity) > linTolSqr)
			        {
				        b._sleepTime = 0.0f;
				        minSleepTime = 0.0f;
			        }
			        else
			        {
				        b._sleepTime += step.dt;
				        minSleepTime = Math.Min(minSleepTime, b._sleepTime);
			        }
		        }

		        if (minSleepTime >= Settings.b2_timeToSleep)
		        {
			        for (int i = 0; i < _bodyCount; ++i)
			        {
				        Body b = _bodies[i];
                        b.SetAwake(false);
			        }
		        }
	        }
        }

	    public void Add(Body body)
	    {
		    Debug.Assert(_bodyCount < _bodyCapacity);
		    body._islandIndex = _bodyCount;
		    _bodies[_bodyCount++] = body;
	    }

	    public void Add(Contact contact)
	    {
            Debug.Assert(_contactCount < _contactCapacity);
            _contacts[_contactCount++] = contact;
        }

	    public void Add(Joint joint)
	    {
		    Debug.Assert(_jointCount < _jointCapacity);
		    _joints[_jointCount++] = joint;
	    }

	    public void Report(ContactConstraint[] constraints)
        {
            if (_listener == null)
	        {
		        return;
	        }

            for (int i = 0; i < _contactCount; ++i)
	        {
		        Contact c = _contacts[i];

                ContactConstraint cc = constraints[i];
        		
		        ContactImpulse impulse = new ContactImpulse();
		        for (int j = 0; j < cc.pointCount; ++j)
		        {
			        impulse.normalImpulses[j] = cc.points[j].normalImpulse;
			        impulse.tangentImpulses[j] = cc.points[j].tangentImpulse;
		        }

		        _listener.PostSolve(c, ref impulse);
	        }
        }

        private ContactSolver _contactSolver = new ContactSolver();
	    public IContactListener _listener;

	    public Body[] _bodies;
        public Contact[] _contacts;
	    public Joint[] _joints;

	    public int _bodyCount;
        public int _contactCount;
	    public int _jointCount;

	    public int _bodyCapacity;
	    public int _contactCapacity;
	    public int _jointCapacity;

	    public int _positionIterationCount;
    };
}
