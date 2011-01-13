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

            float y = yMargin;
            
            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.localPosition = new Vector2 { x = xMargin, y = y };
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
            float x = xMargin;

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.position = new Vector2 { x = x, y = yMargin };
                x += child.bounds.Width + xSpacing;
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
