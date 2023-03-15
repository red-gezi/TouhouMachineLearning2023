using System.Collections.Generic;

namespace TouhouMachineLearningSummary.Model
{
    public class DialogueModel
    {
        public string Tag { get; set; }
        public List<Operation> Operations { get; set; } = new List<Operation>();
        public class Operation
        {
            public Operation(string branch, string chara, string position, string face, Dictionary<string, string> text)
            {
                Branch = branch;
                Chara = chara;
                Position = position;
                Face = face;
                Text = text;
            }
            public string Branch { get; set; }//分支
            public string Chara { get; set; }//角色
            public string Position { get; set; }//立绘位置
            public string Face { get; set; }//面部表情
            public Dictionary<string, string> Text { get; set; }//不同语言的文字
        }
    }
}
