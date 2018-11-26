using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveImage : MonoBehaviour
{
    public SpriteRenderer[] cover; //유니티에서 기본 스프라이트 넣는 배열
    private bool IsCreated = false;
    private SpriteRenderer[] temp; //임시배열

    public GameObject Group;
    public Animator animator;

    // Use this for initialization
    void Start()
    {
        int length = cover.Length;
        temp = new SpriteRenderer[length];
    }

    // Update is called once per frame
    void Update()
    {
        if (IsCreated != true)
            CreateSprite();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            animator.SetTrigger("LmoveTrigger");
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            animator.SetTrigger("RmoveTrigger");

        }
    }

    //이미지 생성해주는 메소즈ㅡ
    bool CreateSprite()
    {
        float vectorX = -22.5f;
        int length = cover.Length;
        for (int i = 0; i < length; i++)
        {
            var go = Instantiate(cover[i], new Vector3(vectorX, 1.2f, 0), Quaternion.identity, Group.transform);
            var newSprite = go.GetComponent<SpriteRenderer>();

            temp[i] = newSprite;
            vectorX += 7.5f;
        }
        return IsCreated = true;
    }

    //왼쪽으로 움직이면 왼쪽으로 1칸씩 옮겨줌
    void Rmove()
    {
        Sprite temp;
        for (int i = 0; i < this.temp.Length - 1; i++)
        {
            if (i == this.temp.Length - 1)
            {
                temp = this.temp[this.temp.Length - 1].sprite;
                this.temp[this.temp.Length - 1].sprite = this.temp[0].sprite;
                this.temp[0].sprite = temp;
            }

            else
            {
                temp = this.temp[i].sprite;
                this.temp[i].sprite = this.temp[i + 1].sprite;
                this.temp[i + 1].sprite = temp;
            }
        }
        Remade();
    }

    //왼쪽으로 움직이면 쪽으로 1칸씩 옮겨줌
    void Lmove()
    {
        Sprite temp;
        for (int i = this.temp.Length - 1; i > 0; i--)
        {
            if (i == 0)
            {
                temp = this.temp[0].sprite;
                this.temp[0].sprite = this.temp[this.temp.Length - 1].sprite;
                this.temp[this.temp.Length - 1].sprite = temp;
            }

            else
            {
                temp = this.temp[i].sprite;
                this.temp[i].sprite = this.temp[i - 1].sprite;
                this.temp[i - 1].sprite = temp;
            }
        }
        Remade();
    }

    void Remade()
    {
        for (int i = 0; i < temp.Length; i++)
        {
            SpriteRenderer j = temp[i];
            cover[i] = j;
        }
    }
}