using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.UI.Controls
{
    public class PanelControl : Control
    {
        protected Rectangle clipRect;

        public PanelControl(int width, int height) : base()
        {
            gameObject.name = "PanelControl";
            clipRect = new Rectangle(0, 0, width, height);
        }
        
        // Position child components in a column, with the given spacing between components
        public void LayoutColumn(float xMargin, float yMargin, float ySpacing)
        {

            float y = yMargin;
            
            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                child.transform.localPosition = new Vector2 { x = xMargin, y = y };
                Debug.Log("child.transform.localPosition: " + child.transform.localPosition + " child.size.y: " + child.size.y);
                y += child.size.y + ySpacing;
            }

            InvalidateAutoSize();
        }

        // Position child components in a row, with the given spacing between components
        public void LayoutRow(float xMargin, float yMargin, float xSpacing)
        {
            float x = xMargin;

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];

                child.position = new Vector2 { x = x, y = yMargin };
                x += child.size.x + xSpacing;
            }

            InvalidateAutoSize();
        }

        protected override void OnChildAdded(int index, Control child)
        {
            base.OnChildAdded(index, child);

            Debug.Log("Setting ClipRect: " + clipRect);
            ((UIRenderer)child.renderer).clipRect = clipRect;
        }
    }
}
