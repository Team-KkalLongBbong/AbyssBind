using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UIScene
{
    enum GameObjects
    {
        Panel
    }


    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.Panel);
        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        for(int i=0; i<8; i++)
        {
            //�⺻ �̸����� null�̰� �Ű������� �̸����� ���µ� �׳� ����� ������ �ش����� �̹� ȣ���Լ����� �����س���
            GameObject item = Managers.UI.MakeSubItem<UI_Inven_Item>(parent: gridPanel.transform).gameObject;

            UI_Inven_Item invenItem = item.GetOrAddComponent<UI_Inven_Item>();
            invenItem.SetInfo($"�����{i}��");
        }
    }
}
