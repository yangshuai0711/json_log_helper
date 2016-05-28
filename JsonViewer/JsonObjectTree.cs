using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;

namespace EPocalipse.Json.Viewer
{
    public enum JsonType { Object, Array, Value };

    class JsonParseError : ApplicationException
    {
        public JsonParseError() : base() { }
        public JsonParseError(string message) : base(message) { }
        protected JsonParseError(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public JsonParseError(string message, Exception innerException) : base(message, innerException) { }

    }


    class Range
    {
        public int start;
        public int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }

    public class JsonObjectTree
    {
        private JsonObject _root;
        

        public static JsonObjectTree Parse(string json)
        {
            //Parse the JSON string
            JavaScriptObject objs;
          
            try
            {
                //FileStream fs1 = new FileStream("Test.txt", FileMode.Create);
                //StreamWriter sw1 = new StreamWriter(fs1);
                //sw1.AutoFlush = true;
                //Console.SetOut(sw1);
                //Console.SetError(sw1);

                objs = splitJson(json);
            }
            catch (Exception e)
            {
                throw new JsonParseError(e.Message, e);
            }
           
            return new JsonObjectTree(objs);
        }

        //add by yangshuai
        public static bool IsJson(string json)
        {
            try
            {
                JavaScriptConvert.DeserializeObject(json);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static JavaScriptObject splitJson(string json)
        {
            Stack<Range> sta = new Stack<Range>();
            LinkedList<Range> rangeList = new LinkedList<Range>();
            char[] str = json.ToCharArray();
            for (int i = 0; i < str.Length; i++)
            {
                Range mark = null;
                if (sta.Count > 0) mark = sta.Peek();
                if (str[i] == '[' || str[i] == '{')
                {
                    sta.Push(new Range(str[i], i));
                }
                else if ((str[i] == ']' && mark != null && mark.start == '[') || (str[i] == '}' && mark != null && mark.start == '{'))
                {
                    Range seg = new Range(mark.end, i + 1);
                    string subStr = json.Substring(seg.start, seg.end - seg.start);
                    if (IsJson(subStr))
                    {
                        addToRange(rangeList,seg);
                        //string posLog = "";
                        //string strLog = "";
                        //LinkedListNode<Range> temp = rangeList.Count>0?rangeList.First:null;
                        //while (temp != null)
                        //{
                        //    string line = json.Substring(temp.Value.start, temp.Value.end - temp.Value.start);
                        //    posLog += "(" + temp.Value.start + "," + temp.Value.end + "),";
                        //    strLog += "【" + line + "】";
                        //    temp = temp.Next;
                       // }
                        //Console.Error.WriteLine("区间列表:" + posLog);
                        //Console.Error.WriteLine("字串列表:" + strLog);
                    }
                    else
                    {
                        //Console.Error.WriteLine("此货不是json:" + subStr);
                    }
                    sta.Pop();
                }
            }

            JavaScriptObject objs = new JavaScriptObject();           
            if (rangeList.Count < 1)
            {
                return objs;
            }
            LinkedListNode<Range> cur =rangeList.Count>0?rangeList.First:null;
            int lastPos = 0;
            while (cur != null)
            {
                object subJson = JavaScriptConvert.DeserializeObject(json.Substring(cur.Value.start, cur.Value.end - cur.Value.start));
                string key = json.Substring(lastPos, cur.Value.start - lastPos);
                while (objs.ContainsKey(key)||key.Length<1) 
                {
                    key = key + " ";
                }
                objs.Add(key, subJson);
                lastPos = cur.Value.end;
                cur = cur.Next;
            }
            return objs;
        }

        private static void addToRange(LinkedList<Range> rangeList,Range range)
        {

            if (rangeList.Count < 1)
            {
                rangeList.AddFirst(range);
                //Console.Out.WriteLine("处理完成！区间为空，添加处理区间（" + range.start + "，" + range.end + "）");
                return;
            }
            LinkedListNode<Range> curRange = rangeList.First;
            while (true)
            {
                if (curRange.Value.start <= range.start && curRange.Value.end >= range.end)
                {//新来的被包含
                    //Console.Out.WriteLine("处理完成！已经包含处理区间无需处理,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                    return;
                }
                else if ((curRange.Next == null && curRange.Value.end <= range.start) || (curRange.Next != null && curRange.Value.end <= range.start && curRange.Next.Value.start >= range.end))
                {//新来的在最后之后或者在间隙中
                   // Console.Out.WriteLine("处理完成！已加入最后,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                    rangeList.AddAfter(curRange, range);
                    return;
                }
                else if (curRange.Previous == null && curRange.Value.start >= range.end)
                {//新来的在最前之前
                   // Console.Out.WriteLine("处理完成！已加入最前,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                    rangeList.AddBefore(curRange, range);
                    return;
                }
                else if (range.start <= curRange.Value.start && range.end >= curRange.Value.end)
                {//被新来的覆盖
                    if (curRange.Previous == null && curRange.Next == null)
                    {//当前剩下唯一区间
                        //Console.Out.WriteLine("处理完成！当前区间被全部覆盖,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                        rangeList.Clear();
                        rangeList.AddFirst(range);
                        return;
                    }
                    else
                    {
                        if (curRange.Next == null)
                        {
                           // Console.Out.WriteLine("处理完成！最后一个区间被覆盖,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                            rangeList.RemoveLast();
                            rangeList.AddLast(range);
                            return;
                        }
                        //Console.Out.WriteLine("当前区间被覆盖,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                        curRange = curRange.Next;
                        rangeList.Remove(curRange.Previous);
                        continue;
                    }
                }
                else if ((range.start > curRange.Value.start && range.start < curRange.Value.end && range.end > curRange.Value.end) || (range.start < curRange.Value.start && range.end < curRange.Value.end && range.end > curRange.Value.start))
                {//交叉情况，异常
                    //Console.Error.WriteLine("区间有交叉异常,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                    return;
                }
                else
                {
                    //Console.Out.WriteLine("进入下一个区间,当前区间（" + curRange.Value.start + "，" + curRange.Value.end + "），处理区间（" + range.start + "，" + range.end + "）");
                }
                curRange = curRange.Next;
                if (curRange == null)
                {
                    break;
                }
            }
        }

        public JsonObjectTree(object rootObject)
        {
            _root = ConvertToObject("解析结果", rootObject);
        }

        private JsonObject ConvertToObject(string id, object jsonObject)
        {
            JsonObject obj = CreateJsonObject(jsonObject);
            obj.Id = id;
            AddChildren(jsonObject, obj);
            return obj;
        }

        private void AddChildren(object jsonObject, JsonObject obj)
        {
            JavaScriptObject javaScriptObject = jsonObject as JavaScriptObject;
            if (javaScriptObject != null)
            {
                foreach (KeyValuePair<string, object> pair in javaScriptObject)
                {
                    obj.Fields.Add(ConvertToObject(pair.Key, pair.Value));
                }
            }
            else
            {
                JavaScriptArray javaScriptArray = jsonObject as JavaScriptArray;
                if (javaScriptArray != null)
                {
                    for (int i = 0; i < javaScriptArray.Count; i++)
                    {
                        obj.Fields.Add(ConvertToObject("[" + i.ToString() + "]", javaScriptArray[i]));
                    }
                }
            }
        }

        private JsonObject CreateJsonObject(object jsonObject)
        {
            JsonObject obj = new JsonObject();
            if (jsonObject is JavaScriptArray)
                obj.JsonType = JsonType.Array;
            else if (jsonObject is JavaScriptObject)
                obj.JsonType = JsonType.Object;
            else
            {
                obj.JsonType = JsonType.Value;
            }
            obj.Value = jsonObject;
            return obj;
        }

        public JsonObject Root
        {
            get
            {
                return _root;
            }
        }

    }
}
