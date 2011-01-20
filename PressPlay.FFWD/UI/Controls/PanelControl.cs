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

        public PanelControl() : base()
        {
            gameObject.name = "PanelControl";
        }

        public void LayoutColumn()
        {
            LayoutColumn(0, 0, 0);
        }

        // Position child components in a column, with the given spacing between components
        public void LayoutColumn(float xMargin, float yMargin, float ySpacing)
        {

            // Parent offset
            Vector2 offset = transform.position;

            float y = yMargin + offset.y;
            
            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.position = new Vector2 { x = offset.x + xMargin, y = y };
                y += child.bounds.Height + ySpacing;
            }

            InvalidateAutoSize();
        }

        public void LayoutRow()
        {
            LayoutRow(0, 0, 0);
        }

        // Position child components in a row, with the given spacing between components
        public void LayoutRow(float xMargin, float yMargin, float xSpacing)
        {

            // Parent offset
            Vector2 offset = transform.position;

            float x = xMargin + offset.x;

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.position = new Vector2 { x = x, y = offset.y + yMargin };

                x += child.bounds.Width + xSpacing;

                //Debug.Log("x: "+x+" child.bounds.Width: " + child.bounds.Width);
            }

            InvalidateAutoSize();
        }


        // Position child components in a row, with the given spacing between components
        public void LayoutRowCentered(float xMargin, float yMargin, float xSpacing)
        {

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
