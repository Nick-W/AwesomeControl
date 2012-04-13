using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeControl
{
    public class MonitorController
    {
        public int MonitorID;
        public Point Offset;
        public Point MonitorSize;
        private ModuleController _module;
        private XController parent;

        public ModuleController Module
        {
            get { return _module; }
            set
            {
                _module = value;
                if (value != null)
                    updateModulePosition();
            }
        }

        public MonitorController(XController parent, int MonitorID, Point Offset, Point MonitorSize)
        {
            this.parent = parent;
            this.Offset = Offset;
            this.MonitorSize = MonitorSize;
            this.MonitorID = MonitorID;
        }

        private void updateModulePosition()
        {
            ProcessManager.SetPos(_module,Offset,MonitorSize);
            //ProcessManager.Fullscreen(_module);
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}x{2}+{3}+{4}", MonitorID, MonitorSize.X, MonitorSize.Y, Offset.X, Offset.Y);
        }
    }
}
