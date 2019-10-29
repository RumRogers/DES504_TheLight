﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class PlayerTimelineController : MonoBehaviour
{
    private enum TimelineIndices
    {
        GRAB_LEFT, GRAB_RIGHT, LAND_LEFT, LAND_RIGHT, DEATH_HEIGHT
    }

    public PlayableDirector director;
    [SerializeField] private List<TimelineAsset> m_timelineAssets;
    [SerializeField] private Animator m_Animator;

    public IEnumerator GrabPipe(Transform character, Transform pipeEnd, Pipe.PipeDirection pipeDirection, GameManager.Callback callback)
    {
        if (character.position.x > pipeEnd.parent.transform.position.x)
        {
            PlayAnimation(character, m_timelineAssets[(int)TimelineIndices.GRAB_LEFT]);
        }
        else
        {
            PlayAnimation(character, m_timelineAssets[(int)TimelineIndices.GRAB_RIGHT]);
        }

        yield return new WaitWhile(() => { return director.state == PlayState.Paused;  });
        yield return new WaitWhile(() => { return character.transform.position.y > pipeEnd.position.y; });
        yield return StartCoroutine(LandFromPipe(character, pipeDirection));

        callback();
    }

    public IEnumerator LandFromPipe(Transform character, Pipe.PipeDirection pipeDirection)
    {
        Quaternion rot = Quaternion.identity;

        if(pipeDirection == Pipe.PipeDirection.Left)
        {
            PlayAnimation(character, m_timelineAssets[(int)TimelineIndices.LAND_LEFT]);
            rot = Quaternion.Euler(0, 90, 0);
            yield return new WaitForSeconds(.5f);
        }
        else
        {
            PlayAnimation(character, m_timelineAssets[(int)TimelineIndices.LAND_RIGHT]);
            rot = Quaternion.Euler(0, -90, 0);
            yield return new WaitForSeconds(.6f);
        }

        //m_timelineAssets[(int)TimelineIndices.LAND_RIGHT].Get
        //yield return new WaitForSeconds((float)m_timelineAssets[(int)TimelineIndices.LAND_RIGHT].duration);
       
        

        character.rotation = rot;
    }

    public IEnumerator Die(Transform child, GameManager.Callback callback = null)
    {
        PlayAnimation(child, m_timelineAssets[(int)TimelineIndices.DEATH_HEIGHT], DirectorWrapMode.None);

        //yield return new WaitWhile(() => { return director.state == PlayState.Paused; });

        if (callback != null)
        {
            callback();
        }

        yield return null;
    }
    private void PlayAnimation(Transform child, TimelineAsset timelineAsset, DirectorWrapMode wrapMode = DirectorWrapMode.None)
    {        
        child.SetParent(null);
        transform.position = child.position;
        child.SetParent(transform);

        director.Play(timelineAsset, wrapMode);
    }
}
