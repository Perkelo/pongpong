public class Room
{
    public long id;
    public string name;
    public short playersCapacity;
    public short playersCurrent;

    public Room(long id, string name, short playersCapacity, short playersCurrent)
    {
        this.id = id;
        this.name = name;
        this.playersCapacity = playersCapacity;
        this.playersCurrent = playersCurrent;
    }
}
