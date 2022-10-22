using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualDesktop;

namespace DesktopHibernate
{
    class DesktopInfo
    {
        public int Index { get; private set; }
        public string Name => GetDesktopName();
        public Guid Id => GetDesktopGuid();
        public IVirtualDesktop DesktopInstance { get; private set; }

        public DesktopInfo(int index, IVirtualDesktop desktopInstance)
        {
            DesktopInstance = desktopInstance;
            Index = index;
        }

        public bool OwnsView(IApplicationView view)
        {
            view.GetVirtualDesktopId(out var ownerDesktopId);

            return ownerDesktopId == Id;
        }

        private string GetDesktopName()
        {
            try
            {
                string registryKey = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VirtualDesktops\Desktops\{" + Id.ToString() + "}";

                object? registryValue = Microsoft.Win32.Registry.GetValue(registryKey, "Name", null);
                return registryValue != null ? (string)registryValue : "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get desktop name for [GUID: {Id}]: {ex}");
                return "";
            }
        }

        private Guid GetDesktopGuid()
        {
            return DesktopInstance.GetId();
        }
    }

}
