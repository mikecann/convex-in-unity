using System;

[Serializable]
public class ConvexMessage
{
  public double _creationTime;
  public string _id;
  public string message;
  public string user;

  public DateTime CreationDateTime => DateTime.UnixEpoch.AddMilliseconds(_creationTime);
}