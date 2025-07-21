using BackEnd;
using System;
using System.Collections.Generic;
using UnityEngine;

// 뒤끝 서버의 공지 사항(Notice) 시스템을 관리하는 클래스
public class BackendNotice : Singleton<BackendNotice>
{
    [SerializeField] private List<Notice> _noticeList = new List<Notice>();    // 현재 로컬에 저장된 우편 리스트

    public List<Notice> GetNoticeList()
    {
        return _noticeList;
    }

    public void NoticeListGet()
    {
        var bro = Backend.Notice.NoticeList();  // 공지 사항 목록 요청

        if (!bro.IsSuccess())
        {
            Debug.LogError("공지 사항 불러오기 중 에러가 발생했습니다.");
            return;
        }

        if (bro.IsSuccess())
        {
            Debug.Log("공지 사항 리스트 불러오기 요청에 성공했습니다. : " + bro);

            LitJson.JsonData jsonList = bro.FlattenRows();

            for (int i = 0; i < jsonList.Count; i++)
            {
                string inDate = jsonList[i]["inDate"].ToString();

                // 중복 방지: inDate가 이미 있는지 확인
                bool alreadyExists = _noticeList.Exists(n => n.InDate == inDate);

                if (alreadyExists)
                {
                    continue; // 이미 추가된 공지라면 건너뜀
                }

                Notice notice = new Notice();

                notice.Title = jsonList[i]["title"].ToString();
                notice.Content = jsonList[i]["content"].ToString();
                notice.PostingDate = DateTime.Parse(jsonList[i]["postingDate"].ToString());
                notice.InDate = jsonList[i]["inDate"].ToString();
                notice.UUid = jsonList[i]["uuid"].ToString();
                notice.IsPublic = jsonList[i]["isPublic"].ToString() == "y" ? true : false;
                notice.Author = jsonList[i]["author"].ToString();

                if (jsonList[i].ContainsKey("imageKey"))
                {
                    notice.ImageKey = "https://s3.ap-northeast-2.amazonaws.com/upload-console.thebackend.io" + jsonList[i]["imageKey"].ToString();
                }
                if (jsonList[i].ContainsKey("linkUrl"))
                {
                    notice.LinkUrl = jsonList[i]["linkUrl"].ToString();
                }
                if (jsonList[i].ContainsKey("linkButtonName"))
                {
                    notice.LinkButtonName = jsonList[i]["linkButtonName"].ToString();
                }

                _noticeList.Add(notice);
            }
        }
    }
}