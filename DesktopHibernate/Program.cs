using System.Diagnostics;
using System.Runtime.InteropServices;
using VirtualDesktop;

namespace DesktopHibernate
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        static void Main(string[] args)
        {
            var views = GetViews();
            var desktops = GetDesktops();

            foreach (var view in views)
            {
                view.GetThumbnailWindow(out var handle);
                view.GetVisibility(out var visibility);

                var desktop = GetViewOwnerDesktop(desktops, view);
                GetWindowThreadProcessId(handle, out var pid);

                var proc = Process.GetProcessById(pid);

                if (desktop == null || proc.ProcessName != "chrome") {
                    continue;
                }

                Console.WriteLine($"Process: {proc.ProcessName}");
                Console.WriteLine($"ProcessID: {proc.Id}");
                Console.WriteLine($"Visibility: {visibility}");
                Console.WriteLine($"Desktop: {desktop.Name}");
            }

            ComResources.ReleaseAll();
        }

        static List<DesktopInfo> GetDesktops()
        {
            var virtualDesktops = ComResources.GetVirtualDesktops();

            return Enumerable
                .Range(0, virtualDesktops.Count)
                .Select(i => new DesktopInfo(i, DesktopManager.GetDesktop(i)))
                .ToList();
        }

        static List<IApplicationView> GetViews()
        {
            return ComResources.GetApplicationViews();
        }

        static DesktopInfo? GetViewOwnerDesktop(List<DesktopInfo> desktops, IApplicationView view)
        {
            return desktops.Find(desktop => desktop.OwnsView(view));
        }
    }
}

