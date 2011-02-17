using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class PanelControl : Control
    {
        public Rectangle clipRect;

        protected float xMargin = 0;
        protected float yMargin = 0;
        protected float xSpacing = 0;
        protected float ySpacing = 0;

        public PanelControl() : base()
        {
            gameObject.name = "PanelControl";
        }

        public void LayoutColumn()
        {
            LayoutColumn(0, 0, 0);
        }

        // Position child components in a column, with the given spacing between components
        public virtual void LayoutColumn(float xMargin, float yMargin, float ySpacing)
        {
            this.xMargin = xMargin;
            this.yMargin = yMargin;
            this.ySpacing = ySpacing;

            // Parent offset
            Vector2 offset = transform.position;

            float y = yMargin + offset.y;

            //Debug.Log("Column size before layout: " + bounds);

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                //child.transform.position = new Vector2 { x = offset.x + xMargin, y = y };
                child.transform.localPosition = new Vector2 { x = offset.x + xMargin, y = y };
                //y += child.bounds.Height + ySpacing;

                //Debug.Log(i + " : " + child.transform.localPosition);

                y += child.bounds.Height + ySpacing;
            }

            InvalidateAutoSize();

            //Debug.Log("Column size after layout: "+bounds);
        }

        public void LayoutRow()
        {
            LayoutRow(0, 0, 0);
        }

        // Position child components in a row, with the given spacing between components
        public virtual void LayoutRow(float xMargin, float yMargin, float xSpacing)
        {
            this.xMargin = xMargin;
            this.yMargin = yMargin;
            this.xSpacing = xSpacing;
            
            // Parent offset
            //Vector2 offset = transform.position;
            Vector2 offset = Vector2.zero;

            float x = xMargin + offset.x;

            //Debug.Log("Row size before layout: " + bounds);

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];

                child.transform.localPosition = new Vector2 { x = x, y = offset.y + yMargin };

                //Debug.Log("x: " + x + " child.bounds.Width: " + child.bounds.Width);
                //x += child.size.x + xSpacing;
                x += child.bounds.Width + xSpacing;
            }

            InvalidateAutoSize();

            //Debug.Log("Row size after layout: "+bounds);
        }


        // Position child components in a row, with the given spacing between components
        public virtual void LayoutRowCentered(float xMargin, float yMargin, float xSpacing)
        {
            this.xMargin = xMargin;
            this.yMargin = yMargin;
            this.xSpacing = xSpacing;

            // Parent offset
            Vector3 offset = transform.position;

            float x = xMargin + offset.x;
            float totalWidth = 0;
            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.position = new Vector3 (x, offset.y, offset.z + yMargin);

                x += child.bounds.Width + xSpacing;

                totalWidth += child.bounds.Width + xSpacing;

                //Debug.Log("x: "+x+" child.bounds.Width: " + child.bounds.Width);
            }
            totalWidth -= xSpacing; //only spacing BETWEEN childs, not after the last one

            //move childs backwards, to center children in panel
            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.position = child.transform.position + new Vector3(-totalWidth * 0.5f, 0, 0);
            }

            InvalidateAutoSize();
        }

        protected override void OnChildAdded(int index, Control child)
        {
            base.OnChildAdded(index, child);

            //Debug.Log("Setting ClipRect: " + clipRect);
            //((UIRenderer)child.renderer).clipRect = clipRect;
        }
    }
}
