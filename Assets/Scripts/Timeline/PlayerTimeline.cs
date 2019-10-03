using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
public class PlayerTimeline : MonoBehaviour
{
    private enum TimelineIndices
    {
        GRAB, LAND
    }

    public PlayableDirector director;
    [SerializeField] private List<TimelineAsset> m_timelineAssets;
    
    public void GrabPipe(Transform character, Pipe.PipeDirection pipeDirection)
    {
        if(pipeDirection == Pipe.PipeDirection.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        character.SetParent(null);
        transform.position = character.position;
        character.SetParent(transform);

        director.Play(m_timelineAssets[(int)TimelineIndices.GRAB]);
    }
}
