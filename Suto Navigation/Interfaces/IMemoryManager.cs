using SutoNavigation.Interfaces;

namespace SutoNavigation.Interfaces
{
    public interface IMemoryManager
    {
        bool AutoMemoryManagementEnabled { get; }

        void EnableAutoMemoryManagement(IMemoryWatcher memWatcher);

        void EnableAutoMemoryManagement(IMemoryWatcher memWatcher, IMemoryReactor reactor);
    }
}