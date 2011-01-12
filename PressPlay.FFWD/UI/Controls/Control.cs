using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.ScreenManager;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class Control : Component, PressPlay.FFWD.Interfaces.IUpdateable
    {

        #region fields
        private Vector2 _size;
        private bool sizeValid = false;
        private bool autoSize = true;
        protected List<Control> children = null;
        public Vector2 drawOffset = Vector2.zero;
        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> OnClickedEvent;
        #endregion

        #region properties
        public Rectangle bounds
        {
            get
            {
                return new Rectangle((int)transform.position.x, (int)transform.position.y, (int)size.x, (int)size.y);
            }
        }

        /// <summary>
        /// Position of this control within its parent control.
        /// </summary>
        public Vector2 position
        {
            get
            {
                return gameObject.transform.localPosition;
            }
            set
            {
                gameObject.transform.localPosition = value;
                if (parent != null)
                {
                    parent.InvalidateAutoSize();
                }
            }
        }

        /// <summary>
        /// Size if this control. See above for a discussion of the layout system.
        /// </summary>
        public Vector2 size
        {
            // Default behavior is for ComputeSize() to determine the size, and then cache it.
            get
            {
                if (!sizeValid)
                {
                    _size = ComputeSize();
                    sizeValid = true;
                }
                return _size;
            }

            // Setting the size overrides whatever ComputeSize() would return, and disables autoSize
            set
            {
                _size = value;
                sizeValid = true;
                autoSize = false;
                if (parent != null)
                {
                    parent.InvalidateAutoSize();
                }
            }
        }

        /// <summary>
        /// Call this method when a control's content changes so that its size needs to be recomputed. This has no
        /// effect if autoSize has been disabled.
        /// </summary>
        protected void InvalidateAutoSize()
        {
            if (autoSize)
            {
                sizeValid = false;
                if (parent != null)
                {
                    parent.InvalidateAutoSize();
                }
            }
        }

        /// <summary>
        /// The control containing this control, if any
        /// </summary>
        public Control parent { get; private set; }

        /// <summary>
        /// Number of child controls of this control
        /// </summary>
        public int childCount { get { return children == null ? 0 : children.Count; } }

        /// <summary>
        /// Indexed access to the children of this control.
        /// </summary>
        public Control this[int childIndex]
        {
            get
            {
                return children[childIndex];
            }
        }
        #endregion


#region constructors
        public Control()
        {
            if (gameObject == null)
            {
                GameObject go = new GameObject("control");
                go.AddComponent(this);
            }
        }
#endregion

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (OnClickedEvent != null)
            {
                OnClickedEvent(this, new PlayerIndexEventArgs(PlayerIndex.One));
            }
        }

        #region Handle input
        /// <summary>
        /// Called once per frame to update the control; override this method if your control requires custom updates.
        /// Call base.Update() to update any child controls.
        /// </summary>
        public virtual void HandleInput(InputState input)
        {

        }
        #endregion

        /// <summary>
        /// Called when the Size property is read and sizeValid is false. Call base.ComputeSize() to compute the
        /// size (actually the lower-right corner) of all child controls.
        /// </summary>
        public virtual Vector2 ComputeSize()
        {
            if (children == null || children.Count == 0)
            {
                return Vector2.zero;
            }
            else
            {
                Vector2 bounds = children[0].position + children[0].size;
                for (int i = 1; i < children.Count; i++)
                {
                    Vector2 corner = children[i].position + children[i].size;
                    bounds.x = Math.Max(bounds.x, corner.y);
                    bounds.y = Math.Max(bounds.x, corner.y);
                }
                return bounds;
            }
        }

        #region IUpdateable Members

        public virtual void Update()
        {

        }

        public void LateUpdate()
        {

        }

        #endregion

        #region Child control API
        public void AddChild(Control child)
        {
            if (child.parent != null)
            {
                child.parent.RemoveChild(child);
            }

            AddChild(child, childCount);
        }

        public void AddChild(Control child, int index)
        {
            if (child.parent != null)
            {
                child.parent.RemoveChild(child);
            }
            child.parent = this;
            child.transform.parent = transform;

            if (children == null)
            {
                children = new List<Control>();
            }

            children.Insert(index, child);
            OnChildAdded(index, child);
        }

        public void RemoveChildAt(int index)
        {
            Control child = children[index];
            child.parent = null;
            child.transform.parent = null;
            children.RemoveAt(index);
            OnChildRemoved(index, child);
        }


        /// <summary>
        /// Remove the given control from this control's list of children.
        /// </summary>
        public void RemoveChild(Control child)
        {
            if (child.parent != this)
                throw new InvalidOperationException();

            RemoveChildAt(children.IndexOf(child));
        }

        /// <summary>
        /// Called after a child control is added to this control. The default behavior is to call InvalidateAutoSize().
        /// </summary>
        protected virtual void OnChildAdded(int index, Control child)
        {
            //InvalidateAutoSize();
        }

        /// <summary>
        /// Called after a child control is removed from this control. The default behavior is to call InvalidateAutoSize().
        /// </summary>
        protected virtual void OnChildRemoved(int index, Control child)
        {
            //InvalidateAutoSize();
        }
        #endregion
    }
}
