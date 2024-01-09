using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebSocketUtils;


public interface ConnectionHandler
{
    public void StartMission(int missionId);

    public void StopMission(int missionId);

    public void ListenToMissionData(UnityAction<Message> call);

    public IEnumerator ListMissions(UnityAction<List<Mission>> onResult, UnityAction<string> onError);

    IEnumerator GetMissionDetail(int missionId, UnityAction onResult, UnityAction<string> onError);
}