using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DamLKK.Views
{
    public enum Operations
    {
        NONE = 0,
        MOVE,
        ROTATE,
        ZOOM,
        DECK_RECT,
        DECK_POLYGON,
        OBSERVE,
        SCROLL_ALL,
        SCROLL_HORZ,
        SCROLL_VERT,
        SCROLL_FREE,
        ROOL_COUNT
    }

    public class OperationsToCursor
    {
        public static Cursor Cursor(Operations currentOp, Cursor magnify)
        {
            switch (currentOp)
            {
                case Operations.ROOL_COUNT:
                    return Cursors.Cross;

                case Operations.SCROLL_HORZ:
                    return Cursors.NoMoveHoriz;

                case Operations.SCROLL_VERT:
                    return Cursors.NoMoveVert;

                case Operations.SCROLL_FREE:
                    return Cursors.NoMove2D;

                case Operations.ROTATE:
                    return Cursors.Default;
                case Operations.DECK_POLYGON:
                case Operations.DECK_RECT:
                case Operations.OBSERVE:
                    return Cursors.Cross;

                case Operations.ZOOM:
                    return magnify;

                default:
                    return Cursors.Default;

            }
        }
    }
}
