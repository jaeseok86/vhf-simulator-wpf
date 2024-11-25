using STRRadar.Libraries.SharedMemories;

namespace VHF_Client.Services.SharedMemories
{
    /// <summary>
    /// 의존성 주입(Dependency Injection)을 위한 SharedMemoryReader를 감싸는 클래스
    /// </summary>
    public class SharedMemoryReaderWrapper : ISharedMemoryReaderWrapper
    {
        public bool HasData<T>(string sharedMemoryName) where T : struct
        {
            return SharedMemoryReader.HasData<T>(sharedMemoryName);
        }

        public T ReadData<T>(string sharedMemoryName) where T : struct
        {
            return SharedMemoryReader.ReadData<T>(sharedMemoryName);
        }
    }
}
