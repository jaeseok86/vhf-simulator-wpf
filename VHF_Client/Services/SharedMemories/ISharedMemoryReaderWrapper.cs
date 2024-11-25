namespace VHF_Client.Services.SharedMemories
{
    /// <summary>
    /// 공유 메모리를 읽어오는 SharedMemoryReader의 Static 메서드를 직접 사용하는 대신(결합도를 낮추기), 해당 동작을 캡슐화한 인터페이스나 추상 클래스를 만들어 추상화
    /// </summary>
    public interface ISharedMemoryReaderWrapper
    {
        bool HasData<T>(string sharedMemoryName) where T : struct;
        T ReadData<T>(string sharedMemoryName) where T : struct;
    }
}
