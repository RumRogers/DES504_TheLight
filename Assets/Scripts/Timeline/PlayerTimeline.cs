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
    [SerializeField] private Transform m_target;
    [SerializeField] private List<TimelineAsset> m_timelineAssets;

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.E))
        {
            GrabPipe();
        }*/
    }
    
    public void GrabPipe(Pipe.PipeDirection pipeDirection)
    {
        if(pipeDirection == Pipe.PipeDirection.Left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        m_target.SetParent(null);
        transform.position = m_target.position;
        m_target.SetParent(transform);

        director.Play(m_timelineAssets[(int)TimelineIndices.GRAB]);
    }
}
