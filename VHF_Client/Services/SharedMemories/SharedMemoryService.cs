namespace VHF_Client.Services.SharedMemories
{
    /// <summary>
    /// 공유 메모리의 데이터를 읽어오는 기능을 가진 클래스. 의존성 주입을 통해 테스트 용이성을 높이고, 필요에 따라 다른 구현체를 주입하여 교체하기 쉽게 구현
    /// </summary>
    public class SharedMemoryService
    {
        private readonly ISharedMemoryReaderWrapper _sharedMemoryReader;

        public SharedMemoryService(ISharedMemoryReaderWrapper sharedMemoryReader)
        {
            _sharedMemoryReader = sharedMemoryReader;
        }

        public bool HasData<T>(string sharedMemoryName) where T : struct
        {
            return _sharedMemoryReader.HasData<T>(sharedMemoryName);
        }

        public T ReadData<T>(string sharedMemoryName) where T : struct
        {
            return _sharedMemoryReader.ReadData<T>(sharedMemoryName);
        }
    }
}
