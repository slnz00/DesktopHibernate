using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VirtualDesktop;

namespace DesktopHibernate
{
    static class ComResources
    {
        private static List<object> comResources = new List<object>();
        public static void ReleaseAll()
        {
            comResources.ForEach(resource => Marshal.ReleaseComObject(resource));
        }

        public static List<IApplicationView> GetApplicationViews()
        {
            var comViews = GetResource(() =>
            {
                DesktopManager.ApplicationViewCollection.GetViews(out IObjectArray _comViews);
                return _comViews;
            });

            return CastResourceTo<IApplicationView>(comViews);
        }

        public static List<IVirtualDesktop> GetVirtualDesktops()
        {
            var comDesktops = GetResource(() =>
            {
                DesktopManager.VirtualDesktopManagerInternal.GetDesktops(out IObjectArray _comDesktops);
                return _comDesktops;
            });

            return CastResourceTo<IVirtualDesktop>(comDesktops);
        }

        private static List<T> CastResourceTo<T>(IObjectArray arr)
        {
            arr.GetCount(out int count);
            var list = new List<T>(count);

            for (int i = 0; i < count; i++)
            {
                arr.GetAt(i, typeof(T).GUID, out object value);
                list.Add((T)value);
            }

            return list;
        }

        private static T GetResource<T>(Func<T> getter)
        {
            var resource = getter();
            if (resource != null)
            {
                comResources.Add(resource);
            }

            return resource;
        }
    }
}
