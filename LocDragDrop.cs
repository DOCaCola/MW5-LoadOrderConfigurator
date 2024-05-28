using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Versioning;

namespace MW5_Mod_Manager
{
    [SupportedOSPlatform("windows")]
    public class ModDragSource : IDragSource
    {
        public ModDragSource()
        {
        }

        public virtual object StartDrag(ObjectListView olv, MouseButtons button, OLVListItem item)
        {
            MainForm.Instance.EnableModListDrop(true);
            return button != MouseButtons.Left ? (object)null : this.CreateDataObject(olv);
        }

        public virtual DragDropEffects GetAllowedEffects(object data)
        {
            return DragDropEffects.All | DragDropEffects.Link;
        }

        public virtual void EndDrag(object dragObject, DragDropEffects effect)
        {
            MainForm.Instance.EnableModListDrop(false);
        }
        protected virtual object CreateDataObject(ObjectListView olv)
        {
            return (object)new OLVDataObject(olv);
        }
    }

    [SupportedOSPlatform("windows")]
    public class ModDropSink : SimpleDropSink
    {
        protected override void CalculateDropTarget(OlvDropEventArgs args, Point pt)
        {
            DropTargetLocation dropTargetLocation = DropTargetLocation.None;
            int num1 = -1;
            int num2 = 0;
            if (this.CanDropOnBackground)
                dropTargetLocation = DropTargetLocation.Background;
            OlvListViewHitTestInfo listViewHitTestInfo1 = this.ListView.OlvHitTest(pt.X, pt.Y);
            if (listViewHitTestInfo1.Item != null && this.CanDropOnItem)
            {
                dropTargetLocation = DropTargetLocation.Item;
                num1 = listViewHitTestInfo1.Item.Index;
                if (listViewHitTestInfo1.SubItem != null && this.CanDropOnSubItem)
                    num2 = listViewHitTestInfo1.Item.SubItems.IndexOf((ListViewItem.ListViewSubItem)listViewHitTestInfo1.SubItem);
            }
            if (this.CanDropBetween && this.ListView.GetItemCount() > 0)
            {
                switch (this.ListView.View)
                {
                    case View.Details:
                    case View.List:
                        if (listViewHitTestInfo1.Item != null)
                        {
                            int num5 = this.ListView.RowHeightEffective / 2;
                            if (pt.Y <= listViewHitTestInfo1.Item.Bounds.Top + num5)
                            {
                                num1 = listViewHitTestInfo1.Item.Index;
                                dropTargetLocation = DropTargetLocation.AboveItem;
                                break;
                            }

                            num1 = listViewHitTestInfo1.Item.Index;
                            dropTargetLocation = DropTargetLocation.BelowItem;
                            break;
                        }
                        OlvListViewHitTestInfo listViewHitTestInfo4 = this.ListView.OlvHitTest(pt.X, pt.Y + 3);
                        if (listViewHitTestInfo4.Item != null)
                        {
                            num1 = listViewHitTestInfo4.Item.Index;
                            dropTargetLocation = DropTargetLocation.AboveItem;
                            break;
                        }
                        OlvListViewHitTestInfo listViewHitTestInfo5 = this.ListView.OlvHitTest(pt.X, pt.Y - 3);
                        if (listViewHitTestInfo5.Item != null)
                        {
                            num1 = listViewHitTestInfo5.Item.Index;
                            dropTargetLocation = DropTargetLocation.BelowItem;
                            break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            args.DropTargetLocation = dropTargetLocation;
            args.DropTargetIndex = num1;
            args.DropTargetSubItemIndex = num2;
        }

        protected override void DrawBetweenLine(Graphics g, int x1, int y1, int x2, int y2)
        {
            using (Brush b = (Brush)new SolidBrush(this.FeedbackColor))
            {
                DrawClosedFigure(g, b, RightPointingArrow(x1, y1));
                DrawClosedFigure(g, b, LeftPointingArrow(x2, y2));
            }
            using (Pen pen = new Pen(this.FeedbackColor, 3f))
                g.DrawLine(pen, x1, y1, x2, y2);
        }

        private static void DrawClosedFigure(Graphics g, Brush b, Point[] pts)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.StartFigure();
                path.AddLines(pts);
                path.CloseFigure();
                g.FillPath(b, path);
            }
        }

        private static Point[] RightPointingArrow(int x, int y)
        {
            return new Point[3]
            {
                new Point(x, y - 6),
                new Point(x, y + 6),
                new Point(x + 6, y)
            };
        }

        private static Point[] LeftPointingArrow(int x, int y)
        {
            return new Point[3]
            {
                new Point(x, y - 6),
                new Point(x, y + 6),
                new Point(x - 6, y)
            };
        }

        protected override Rectangle CalculateDropTargetRectangle(OLVListItem item, int subItem)
        {
            if (subItem > 0)
                return item.SubItems[subItem].Bounds;
            Rectangle cellTextBounds = item.Bounds;
            if (item.IndentCount > 0)
            {
                int num = this.ListView.SmallImageSize.Width * item.IndentCount;
                cellTextBounds.X += num;
                cellTextBounds.Width -= num;
            }
            return cellTextBounds;
        }
    }
}
