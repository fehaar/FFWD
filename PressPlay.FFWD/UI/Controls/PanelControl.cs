using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.UI.Controls
{
    public class PanelControl : Control
    {
        // Position child components in a column, with the given spacing between components
        public void LayoutColumn(float xMargin, float yMargin, float ySpacing)
        {
            float y = yMargin;

            for (int i = 0; i < childCount; i++)
            {
                Control child = this[i];
                gameObject.transform.localPosition = new Vector2 { x = xMargin, y = y };
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
    }
}
