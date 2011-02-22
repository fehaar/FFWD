using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace PressPlay.FFWD
{
    class ComponentProfiler
    {
        private List<ComponentUpdateProfile> componentUpdateProfiles = new List<ComponentUpdateProfile>();

        private ComponentUpdateProfile currentUpdateProfile;
        
        

        public long totalTicks = 0;
        public float totalMilliseconds {
            get {
                return (totalTicks / Stopwatch.Frequency) * 1000f;
            }
        }

        private ComponentUpdateProfile GetComponentProfileFromList(Component _component)
        {
            for (int i = 0; i < componentUpdateProfiles.Count; i++)
            {
                if (componentUpdateProfiles[i].component.GetType() == _component.GetType())
                {
                    return componentUpdateProfiles[i];
                }
            }
            ComponentUpdateProfile updateProfile = new ComponentUpdateProfile(_component);
            componentUpdateProfiles.Add(updateProfile);
            
            return updateProfile;
        }

        public void StartUpdateCall(Component _component)
        {
            currentUpdateProfile = GetComponentProfileFromList(_component);
            currentUpdateProfile.updateCalls++;

            currentUpdateProfile.updateStopwatch.Start();
        }

        public void StartFixedUpdateCall(Component _component)
        {
            currentUpdateProfile = GetComponentProfileFromList(_component);
            currentUpdateProfile.fixedUpdateCalls++;

            currentUpdateProfile.updateStopwatch.Start();
        }

        public void StartLateUpdateCall(Component _component)
        {
            currentUpdateProfile = GetComponentProfileFromList(_component);
            currentUpdateProfile.lateUpdateCalls++;

            currentUpdateProfile.updateStopwatch.Start();
        }

        public void EndUpdateCall()
        {
            currentUpdateProfile.updateStopwatch.Stop();
        }

        public void EndFixedUpdateCall()
        {
            currentUpdateProfile.updateStopwatch.Stop();
        }

        public void EndLateUpdateCall()
        {
            currentUpdateProfile.updateStopwatch.Stop();
        }

        public void FlushData()
        {
            totalTicks = 0;
            componentUpdateProfiles.Clear();
        }

        public List<ComponentUpdateProfile> Sort()
        {
            componentUpdateProfiles.Sort();

            return componentUpdateProfiles;
        }

        public ComponentUpdateProfile GetWorst()
        {
            ComponentUpdateProfile worst = new ComponentUpdateProfile(null);

            /*for (int i = 0; i < componentUpdateProfiles.Count; i++)
            {
                if (componentUpdateProfiles[i].totalTicks > worst.totalTicks)
                {
                    worst = componentUpdateProfiles[i];
                }
            }*/

            //if (worst.name == null)
            //{
            //    Debug.Log("whuaa");
            //}
            if (componentUpdateProfiles.Count == 0) { return new ComponentUpdateProfile(); }
            return componentUpdateProfiles[0];
        }
    }

    struct ComponentUpdateProfile : IComparable<ComponentUpdateProfile>
    {
        public Component component;
        public string name
        {
            get 
            {
                if (component == null)
                { return null; }
                return component.name;
            }
        }
        public int updateCalls;
        public int lateUpdateCalls;
        public int fixedUpdateCalls;

        public Stopwatch updateStopwatch;
        
        public float totalMilliseconds {
            get {
                return (totalTicks / Stopwatch.Frequency) * 1000f;
            }
        }

        public long totalTicks 
        {
            get {
                if (updateStopwatch == null)
                {
                    return 0;
                }

                return updateStopwatch.ElapsedTicks; 
            }
        }

        public ComponentUpdateProfile(Component component)
        {
            this.component = component;
            updateCalls = 0;
            lateUpdateCalls = 0;
            fixedUpdateCalls = 0;
            updateStopwatch = new Stopwatch();
        }

        public void Flush()
        {
            updateCalls = 0;
            lateUpdateCalls = 0;
            fixedUpdateCalls = 0;
            updateStopwatch.Reset();
        }

        public int CompareTo(ComponentUpdateProfile other)
        {
            return (int)other.totalTicks - (int)totalTicks;
        }

        public override  string ToString()
        {
            if (name == null)
            {
                return "null " + totalMilliseconds.ToString();
            }

            return name + " " + totalMilliseconds.ToString() + " ms  " + totalTicks + " ticks";
        }
    }
}
