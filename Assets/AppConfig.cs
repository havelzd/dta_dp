
using UnityEngine;


[CreateAssetMenu(fileName = "ServerConfig", menuName = "Custom/Server Config")]
public class AppConfig : ScriptableObject
{
    public string serverHost = "192.168.0.115";
    public string serverPort = "8080";
    public bool isDebug = true;
}
    

