using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;

namespace CSALExtractAndConvertSpeechTool
{
    public partial class Form1 : Form
    {
        DirectoryInfo dir;
        ArrayList fileNames;
        string getCondition;
      
        string currentFile = "";
        bool isChanged = false;
        bool isText = false;
        List<string[]> getAddressInfo = new List<string[]>();
        List<string> getFilesName = new List<string>();
        List<string> getDirectoryName = new List<string>();
        List<string> getSpeechSentence = new List<string>();
        int CPlayNum = 0;
        int JPlayNum = 0;
        string playAttr;
        ArrayList alAMPR = new ArrayList();
        ArrayList alBMPR = new ArrayList();
        public Form1()
        {
            InitializeComponent();
        }

      

      

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.folderBrowserDialog1.ShowDialog().Equals(DialogResult.OK)) return;
            dir = new DirectoryInfo(this.folderBrowserDialog1.SelectedPath);
            this.fileNames = new ArrayList();
            this.listBox1.Items.Clear();
            getCondition = dir.Name;
            this.AddDirectory(dir);
        }
        private void AddDirectory(DirectoryInfo dir)
        {
            this.AddFiles(dir);
            DirectoryInfo[] subdir = dir.GetDirectories();
            if (subdir != null && subdir.Length > 0)
            {
                for (int i = 0; i < subdir.Length; i++)
                {
                    this.AddDirectory(subdir[i]);
                }
            }
        }
        private void AddFiles(DirectoryInfo dir)
        {

            FileInfo[] files = dir.GetFiles("*.xml");

            string folderName = dir.Name.ToString();
            for (int i = 0; i < files.Length; i++)
            {
                this.listBox1.Items.Add(files[i].FullName.ToString());
                getFilesName.Add(files[i].FullName.ToString());
                getDirectoryName.Add(files[i].DirectoryName.ToString());
              

            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((!this.currentFile.Equals("")) && isChanged && isText)
            {
                if (MessageBox.Show("Your text has changed. Do you want to save the changes?", "Save file", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
                {
                    FileStream fs = new FileStream(currentFile, FileMode.Create, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(this.richTextBox1.Text);
                    sw.Close();
                    fs.Close();
                }
            }
            this.currentFile = (string)this.getFilesName[this.listBox1.SelectedIndex];
            string userID = listBox1.Text;
            string fullPath = getFilesName[this.listBox1.SelectedIndex];
            this.richTextBox1.Text = this.parseXML(this.currentFile, fullPath);
            isChanged = false;
            isText = true;
            this.label1.Text = this.currentFile.Substring(this.currentFile.LastIndexOf("\\") + 1);
        }




        private ArrayList getSentence2(string sentence)
        {
            ArrayList al1 = new ArrayList();
            if (sentence.Length > 1)
            {
                bool find = sentence.ToLower().Contains("_user_");
                if (find != true)
                {
                   
                     al1.Add(sentence);
                     return al1;
                 
                    
                }
                else
                {
                    
                    string[] sArray = Regex.Split(sentence, "_", RegexOptions.IgnoreCase);
                     ArrayList al = new ArrayList(sArray);
                    for (int i = 0; i < al.Count; i++)
                    {
                        string getSingalValue = al[i].ToString().Trim();
                        getSingalValue = getSingalValue.Replace("user", "_user_").Trim().ToString();
                        al[i] = getSingalValue;
                        if (getSingalValue.Length < 2)
                        {
                            al.RemoveAt(i);
                        }
                     
                    }

                    return al;
                  
                }


            }
            else
            {
                return al1;
            
            }
        }

        private bool getSentence(string sentence)
        {
            if (sentence.Length > 1)
            {
                bool find = sentence.ToLower().Contains("_user_");
                if (find != true)
                {
                    return true;
                }
                else
                {
                    return true;
                }


            }
            else
            {
                return false;
            }
          

        }

        private string newAPlayAttr(ArrayList al, string agent)
        {
          
            playAttr = "";
            if (al.Count > 1)
            {
                for (int i = 0; i < al.Count; i++)
                {

                    if (al[i].ToString().Trim() != "_user_" && al[i].ToString().Trim() !="user")
                    {
                        CPlayNum++;
                        if (i == al.Count - 1)
                        {
                            playAttr += "A" + CPlayNum;
                            alAMPR.Add(al[i].ToString());
                        }
                        else
                        {
                            playAttr += "A" + CPlayNum + ",";
                            alAMPR.Add(al[i].ToString());
                        }
                       
                    }
                    else if (al[i].ToString().Trim() == "user" || al[i].ToString().Trim() == "_user_" && i != al.Count - 1)
                    {
                        playAttr += "_user_,";

                    }
                    if (i == al.Count - 1)
                    {
                        if (al[i].ToString().Trim() == "user" || al[i].ToString().Trim() == "_user_")
                        {
                            playAttr += "_user_";
                        }

                    }

                }
            
            }
            else
            {
                CPlayNum++;
                playAttr = "A" + CPlayNum;
                alAMPR.Add(al[0].ToString());
               
            }
            return playAttr;
         


        }
        private string newBPlayAttr(ArrayList al, string agent)
        {
            playAttr = "";
            if (al.Count > 1)
            {
                for (int i = 0; i < al.Count; i++)
                {

                    if (al[i].ToString().Trim() != "_user_" && al[i].ToString().Trim() != "user")
                    {
                        JPlayNum++;

                        if (i == al.Count - 1)
                        {
                            playAttr += "B" + JPlayNum;
                            alBMPR.Add(al[i].ToString());
                        }
                        else
                        {
                            playAttr += "B" + JPlayNum + ",";
                            alBMPR.Add(al[i].ToString());
                        }
                    }
                    else if (al[i].ToString().Trim() == "user" || al[i].ToString().Trim() == "_user_" && i != al.Count - 1)
                    {
                        playAttr += "_user_,";
                       
                    }
                    if ( i == al.Count - 1)
                    {
                        if (al[i].ToString().Trim() == "user" || al[i].ToString().Trim() == "_user_")
                        {
                            playAttr += "_user_";
                        }
                       
                    }

                }

                

            }
            else
            {
             
              JPlayNum++;
              playAttr = "B" + JPlayNum;
              alBMPR.Add(al[0].ToString());
             


            }
            return playAttr;



        }
        private void button1_Click(object sender, EventArgs e)
        {

            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(parseXML);
            Thread myThread = new Thread(ParStart);
            object o = "changeXML";
            myThread.Start(o);


        }

     

        private void parseXML(object ParObject)
        {
            getSpeechSentence.Clear();
            string getCommand = ParObject.ToString();
            for (int m = 0; m < getFilesName.Count; m++)
            {
                CPlayNum = 0;
                JPlayNum = 0;
                string path = @getDirectoryName[m];
                StringBuilder sb = new StringBuilder();

                XmlDocument doc = new XmlDocument();
                doc.Load(@getFilesName[m]);

                XmlNode xn = doc.SelectSingleNode("AutoTutorScript");

                XmlNodeList xnl = xn.ChildNodes;

                foreach (XmlNode xn1 in xnl)
                {
                    if (xn1.Name.ToString() == "Agents" || xn1.Name.ToString() == "RigidPacks" || xn1.Name.ToString() == "TutoringPacks")
                    {
                        XmlNodeList xnl2 = xn1.ChildNodes;


                        foreach (XmlNode xn2 in xnl2)
                        {
                            XmlNodeList xnl3 = xn2.ChildNodes;

                            if (xn2.Name.ToString() == "Agent")
                            {
                                XmlElement getAgent = (XmlElement)xn2;
                                string agent = getAgent.GetAttribute("name").ToString();
                                foreach (XmlNode xn3 in xnl3)
                                {
                                    if (xn3.Name.ToString() == "SpeechCan")
                                    {
                                        XmlNodeList xnl4 = xn3.ChildNodes;
                                        foreach (XmlNode xn4 in xnl4)
                                        {
                                            if (xn4.Name.ToString() == "Item")
                                            {
                                                XmlElement xe = (XmlElement)xn4;
                                                string text = xe.GetAttribute("text").ToString();
                                                string speech = xe.GetAttribute("speech").ToString();
                                                bool sentence = getSentence(speech);
                                                ArrayList al = getSentence2(speech);
                                                if (sentence == true)
                                                {

                                                    if (agent == "Cristina")
                                                    {
                                                        if (al.Count > 0)
                                                        {
                                                            string playAttr = newAPlayAttr(al, agent);
                                                            xe.SetAttribute("play", playAttr);
                                                            getSpeechSentence.Add(agent + ":" + playAttr);
                                                        
                                                        }
                                                      

                                                       
                                                    
                                                    }
                                                    else if (agent == "Jordan")
                                                    {
                                                        if (al.Count > 0)
                                                        {
                                                            string playAttr = newBPlayAttr(al, agent);
                                                            xe.SetAttribute("play", playAttr);
                                                            getSpeechSentence.Add(agent + ":" + playAttr);

                                                        }
                                                    }

                                                  

                                                }
                                                else
                                                {

                                                    sentence = getSentence(text);
                                                    al = getSentence2(text);
                                                    if (sentence == true)
                                                    {
                                                        if (agent == "Cristina")
                                                        {
                                                            if (al.Count > 0)
                                                            {
                                                                string playAttr = newAPlayAttr(al, agent);
                                                                xe.SetAttribute("play", playAttr);
                                                                getSpeechSentence.Add(agent + ":" + playAttr);
                                                            }
                                                           

                                                        }
                                                        else if (agent == "Jordan")
                                                        {
                                                            if (al.Count > 0)
                                                            {
                                                                string playAttr = newBPlayAttr(al, agent);
                                                                xe.SetAttribute("play", playAttr);
                                                                getSpeechSentence.Add(agent + ":" + playAttr);

                                                            }
                                                        }
                                                     
                                                    }
                                                }

                                            }
                                        }


                                    }

                                }
                            }
                            else if (xn2.Name.ToString() == "RigidPack")
                            {
                                XmlNodeList xnl5 = xn2.ChildNodes;
                                foreach (XmlNode xn5 in xnl5)
                                {
                                    XmlElement xe = (XmlElement)xn5;
                                    string agent = xe.GetAttribute("agent").ToString();
                                    string text = xe.GetAttribute("text").ToString();
                                    string speech = xe.GetAttribute("speech").ToString();
                                    bool sentence = getSentence(speech);
                                    ArrayList al = getSentence2(speech);
                                    if (sentence == true)
                                    {

                                        if (agent == "Cristina")
                                        {
                                            if (al.Count > 0)
                                            {
                                                string playAttr = newAPlayAttr(al, agent);
                                                xe.SetAttribute("play", playAttr);
                                                getSpeechSentence.Add(agent + ":" + playAttr);
                                            }

                                        }
                                        else if (agent == "Jordan")
                                        {
                                            if (al.Count > 0)
                                            {
                                                string playAttr = newBPlayAttr(al, agent);
                                                xe.SetAttribute("play", playAttr);
                                                getSpeechSentence.Add(agent + ":" + playAttr);

                                            }
                                        }
                                      

                                    }
                                    else
                                    {

                                        sentence = getSentence(text);
                                        al = getSentence2(text);
                                        if (sentence == true)
                                        {
                                            if (agent == "Cristina")
                                            {
                                                if (al.Count > 0)
                                                {
                                                    string playAttr = newAPlayAttr(al, agent);
                                                    xe.SetAttribute("play", playAttr);
                                                    getSpeechSentence.Add(agent + ":" + playAttr);
                                                }

                                            }
                                            else if (agent == "Jordan")
                                            {
                                                if (al.Count > 0)
                                                {
                                                    string playAttr = newBPlayAttr(al, agent);
                                                    xe.SetAttribute("play", playAttr);
                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                }
                                            }
                                          
                                        }
                                    }
                                }


                            }
                            else if (xn2.Name.ToString() == "TutoringPack")
                            {
                                XmlNodeList TP1 = xn2.ChildNodes;
                                foreach (XmlNode TPnote1 in TP1)
                                {
                                    if (TPnote1.Name.ToString() == "Questions")
                                    {
                                        XmlNodeList TP2 = TPnote1.ChildNodes;
                                        {
                                            foreach (XmlNode TPnote2 in TP2)
                                            {
                                                if (TPnote2.Name.ToString() == "Question")
                                                {
                                                    XmlElement xe = (XmlElement)TPnote2;
                                                    string agent = xe.GetAttribute("agent").ToString();
                                                    string text = xe.GetAttribute("text").ToString();
                                                    string speech = xe.GetAttribute("speech").ToString();
                                                    bool sentence = getSentence(speech);
                                                    ArrayList al = getSentence2(speech);
                                                    if (sentence == true)
                                                    {

                                                        if (agent == "Cristina")
                                                        {
                                                            if (al.Count > 0)
                                                            {
                                                                string playAttr = newAPlayAttr(al, agent);
                                                                xe.SetAttribute("play", playAttr);
                                                                getSpeechSentence.Add(agent + ":" + playAttr);
                                                            }

                                                        }
                                                        else if (agent == "Jordan")
                                                        {
                                                            if (al.Count > 0)
                                                            {
                                                                string playAttr = newBPlayAttr(al, agent);
                                                                xe.SetAttribute("play", playAttr);
                                                                getSpeechSentence.Add(agent + ":" + playAttr);

                                                            }
                                                        }
                                                      

                                                    }
                                                    else
                                                    {

                                                        sentence = getSentence(text);
                                                        al = getSentence2(text);
                                                        if (sentence == true)
                                                        {
                                                            if (agent == "Cristina")
                                                            {
                                                                if (al.Count > 0)
                                                                {
                                                                    string playAttr = newAPlayAttr(al, agent);
                                                                    xe.SetAttribute("play", playAttr);
                                                                    getSpeechSentence.Add(agent + ":" + playAttr);
                                                                }
                                                            }
                                                           
                                                            else if (agent == "Jordan")
                                                            {
                                                                if (al.Count > 0)
                                                                {
                                                                    string playAttr = newBPlayAttr(al, agent);
                                                                    xe.SetAttribute("play", playAttr);
                                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                                }
                                                            }
                                                           
                                                        }
                                                    }

                                                    if (TPnote2.HasChildNodes == true)
                                                    {
                                                        XmlNodeList TP3 = TPnote2.ChildNodes;
                                                        foreach (XmlNode TPnote3 in TP3)
                                                        {
                                                            if (TPnote3.HasChildNodes == true && TPnote3.Name.ToString() == "Answers")
                                                            {
                                                                XmlNodeList TP4 = TPnote3.ChildNodes;
                                                                foreach (XmlNode TPnote4 in TP4)
                                                                {

                                                                    XmlElement xe1 = (XmlElement)TPnote4;
                                                                    agent = xe1.GetAttribute("agent").ToString();
                                                                    text = xe1.GetAttribute("text").ToString();
                                                                    speech = xe1.GetAttribute("speech").ToString();
                                                                    sentence = getSentence(speech);
                                                                    al = getSentence2(speech);
                                                                    if (sentence == true)
                                                                    {
                                                                        if (agent == "Cristina")
                                                                        {
                                                                            if (al.Count > 0)
                                                                            {
                                                                                string playAttr = newAPlayAttr(al, agent);
                                                                                xe1.SetAttribute("play", playAttr);
                                                                                getSpeechSentence.Add(agent + ":" + playAttr);
                                                                            }

                                                                        }
                                                                        else if (agent == "Jordan")
                                                                        {
                                                                            if (al.Count > 0)
                                                                            {
                                                                                string playAttr = newBPlayAttr(al, agent);
                                                                                xe1.SetAttribute("play", playAttr);
                                                                                getSpeechSentence.Add(agent + ":" + playAttr);

                                                                            }
                                                                        }
                                                                      

                                                                    }
                                                                    else
                                                                    {

                                                                        sentence = getSentence(text);
                                                                        al = getSentence2(text);
                                                                        if (sentence == true)
                                                                        {
                                                                            if (agent == "Cristina")
                                                                            {
                                                                                if (al.Count > 0)
                                                                                {
                                                                                    string playAttr = newAPlayAttr(al, agent);
                                                                                    xe1.SetAttribute("play", playAttr);
                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                }

                                                                            }
                                                                            else if (agent == "Jordan")
                                                                            {
                                                                                if (al.Count > 0)
                                                                                {
                                                                                    string playAttr = newBPlayAttr(al, agent);
                                                                                    xe1.SetAttribute("play", playAttr);
                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                }
                                                                            }
                                                                          
                                                                        }
                                                                    }
                                                                }

                                                            }


                                                        }

                                                    }
                                                }

                                            }

                                        }
                                    }


                                    else if (TPnote1.Name.ToString() == "Expectations")
                                    {
                                        XmlNodeList ex1 = TPnote1.ChildNodes;
                                        foreach (XmlNode exnote1 in ex1)
                                        {
                                            if (exnote1.HasChildNodes == true)
                                            {
                                                if (exnote1.Name.ToString() == "Expectation")
                                                {
                                                    XmlNodeList ex2 = exnote1.ChildNodes;
                                                    foreach (XmlNode exnote2 in ex2)
                                                    {
                                                        if (exnote2.HasChildNodes == true)
                                                        {
                                                            if (exnote2.Name.ToString() == "Answers" || exnote2.Name.ToString() == "Hints" || exnote2.Name.ToString() == "Prompts")
                                                            {


                                                                XmlNodeList ex3 = exnote2.ChildNodes;
                                                                foreach (XmlNode exnote3 in ex3)
                                                                {
                                                                    if (exnote3.Name.ToString() == "Answer" || exnote3.Name.ToString() == "Hint" || exnote3.Name.ToString() == "Prompt")
                                                                    {
                                                                        XmlElement xe1 = (XmlElement)exnote3;
                                                                        string agent = xe1.GetAttribute("agent").ToString();
                                                                        string text = xe1.GetAttribute("text").ToString();
                                                                        string speech = xe1.GetAttribute("speech").ToString();
                                                                        bool sentence = getSentence(speech);
                                                                        ArrayList al = getSentence2(speech);
                                                                        if (sentence == true)
                                                                        {
                                                                            if (agent == "Cristina")
                                                                            {
                                                                                if (al.Count > 0)
                                                                                {
                                                                                    string playAttr = newAPlayAttr(al, agent);
                                                                                    xe1.SetAttribute("play", playAttr);
                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                }

                                                                            }
                                                                            else if (agent == "Jordan")
                                                                            {
                                                                                if (al.Count > 0)
                                                                                {
                                                                                    string playAttr = newBPlayAttr(al, agent);
                                                                                    xe1.SetAttribute("play", playAttr);
                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                }
                                                                            }
                                                                           


                                                                        }
                                                                        else
                                                                        {

                                                                            sentence = getSentence(text);
                                                                            al = getSentence2(text);
                                                                            if (sentence == true)
                                                                            {
                                                                                if (agent == "Cristina")
                                                                                {
                                                                                    if (al.Count > 0)
                                                                                    {
                                                                                        string playAttr = newAPlayAttr(al, agent);
                                                                                        xe1.SetAttribute("play", playAttr);
                                                                                        getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                    }


                                                                                }
                                                                                else if (agent == "Jordan")
                                                                                {
                                                                                    if (al.Count > 0)
                                                                                    {
                                                                                        string playAttr = newBPlayAttr(al, agent);
                                                                                        xe1.SetAttribute("play", playAttr);
                                                                                        getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                    }
                                                                                }
                                                                               
                                                                            }
                                                                        }

                                                                        if (exnote3.HasChildNodes == true)
                                                                        {

                                                                            XmlNodeList ex4 = exnote3.ChildNodes;
                                                                            foreach (XmlNode exnote4 in ex4)
                                                                            {
                                                                                XmlNodeList ex5 = exnote4.ChildNodes;
                                                                                XmlElement xe2 = (XmlElement)exnote4;
                                                                                agent = xe2.GetAttribute("agent").ToString();
                                                                                text = xe2.GetAttribute("text").ToString();
                                                                                speech = xe2.GetAttribute("speech").ToString();
                                                                                sentence = getSentence(speech);
                                                                                al = getSentence2(speech);
                                                                                if (sentence == true)
                                                                                {

                                                                                    if (agent == "Cristina")
                                                                                    {
                                                                                        if (al.Count > 0)
                                                                                        {
                                                                                            string playAttr = newAPlayAttr(al, agent);
                                                                                            xe2.SetAttribute("play", playAttr);
                                                                                            getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                        }

                                                                                    }
                                                                                    else if (agent == "Jordan")
                                                                                    {
                                                                                        if (al.Count > 0)
                                                                                        {
                                                                                            string playAttr = newBPlayAttr(al, agent);
                                                                                            xe2.SetAttribute("play", playAttr);
                                                                                            getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                        }
                                                                                    }
                                                                                  

                                                                                }
                                                                                else
                                                                                {

                                                                                    sentence = getSentence(text);
                                                                                    al = getSentence2(text);
                                                                                    if (sentence == true)
                                                                                    {
                                                                                        if (agent == "Cristina")
                                                                                        {
                                                                                            if (al.Count > 0)
                                                                                            {
                                                                                                string playAttr = newAPlayAttr(al, agent);
                                                                                                xe2.SetAttribute("play", playAttr);
                                                                                                getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                            }

                                                                                        }
                                                                                        else if (agent == "Jordan")
                                                                                        {
                                                                                            if (al.Count > 0)
                                                                                            {
                                                                                                string playAttr = newBPlayAttr(al, agent);
                                                                                                xe2.SetAttribute("play", playAttr);
                                                                                                getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                            }
                                                                                        }
                                                                                        
                                                                                    }
                                                                                }

                                                                                if (exnote4.HasChildNodes == true)
                                                                                {
                                                                                    foreach (XmlNode exnote5 in ex5)
                                                                                    {

                                                                                        XmlElement xe3 = (XmlElement)exnote5;
                                                                                        agent = xe3.GetAttribute("agent").ToString();
                                                                                        text = xe3.GetAttribute("text").ToString();
                                                                                        speech = xe3.GetAttribute("speech").ToString();
                                                                                        sentence = getSentence(speech);
                                                                                        al = getSentence2(speech);
                                                                                        if (sentence == true)
                                                                                        {

                                                                                            if (agent == "Cristina")
                                                                                            {
                                                                                                if (al.Count > 0)
                                                                                                {
                                                                                                    string playAttr = newAPlayAttr(al, agent);
                                                                                                    xe3.SetAttribute("play", playAttr);
                                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                                }

                                                                                            }
                                                                                            else if (agent == "Jordan")
                                                                                            {
                                                                                                if (al.Count > 0)
                                                                                                {
                                                                                                    string playAttr = newBPlayAttr(al, agent);
                                                                                                    xe3.SetAttribute("play", playAttr);
                                                                                                    getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                                }
                                                                                            }
                                                                                           

                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            sentence = getSentence(text);
                                                                                            al = getSentence2(text);
                                                                                            if (sentence == true)
                                                                                            {
                                                                                                if (agent == "Cristina")
                                                                                                {
                                                                                                    if (al.Count > 0)
                                                                                                    {
                                                                                                        string playAttr = newAPlayAttr(al, agent);
                                                                                                        xe3.SetAttribute("play", playAttr);
                                                                                                        getSpeechSentence.Add(agent + ":" + playAttr);
                                                                                                    }

                                                                                                }
                                                                                                else if (agent == "Jordan")
                                                                                                {
                                                                                                    if (al.Count > 0)
                                                                                                    {
                                                                                                        string playAttr = newBPlayAttr(al, agent);
                                                                                                        xe3.SetAttribute("play", playAttr);
                                                                                                        getSpeechSentence.Add(agent + ":" + playAttr);

                                                                                                    }
                                                                                                }
                                                                                                
                                                                                            }
                                                                                        }

                                                                                    }

                                                                                }




                                                                            }


                                                                        }

                                                                    }
                                                                }


                                                            }
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }


                                }

                            }

                        }

                    }
                }
                if(getCommand=="changeXML")

                {
                 doc.Save(@getFilesName[m]);
                }

                else if (getCommand == "saveSentences")
                {

                    for (int v = 0; v < getSpeechSentence.Count; v++)
                    {

                        string SpeechSentence = getSpeechSentence[v].ToString();


                        FileStream fsw = new FileStream(@path + "\\sentences.txt", FileMode.Append, FileAccess.Write);
                        StreamWriter m_streamWriter = new StreamWriter(fsw, Encoding.Default);

                        // string output = outputTex[v].ToString();
                        m_streamWriter.Flush();
                        m_streamWriter.WriteLine(SpeechSentence);

                        //关闭此文件  m_streamWriter.Flush ( ) ;
                        m_streamWriter.Close();

                    }

                    getSpeechSentence.Clear();
                
                }
                else if (getCommand == "insertToMPR")
                {
                    GenerateXMLFileToA(alAMPR);
                    GenerateXMLFileToB(alBMPR);
                   // insertToMPR(alAMPR);
                    getSpeechSentence.Clear();
                }
               


            }
            MessageBox.Show("successful save! ");
        
        
        }
        private void GenerateXMLFileToA(ArrayList alAMPR)
        {
            int count = 0;
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);
            XmlElement start = doc.CreateElement("start");
            doc.AppendChild(start);
            for (int v = 0; v < alAMPR.Count; v++)
            {
                count++;
                string SpeechSentence = alAMPR[v].ToString();
               // string[] SentenceInfo = SpeechSentence.Split(":".ToCharArray());
                XmlElement message = doc.CreateElement("message");

               
                string T = "A" + count.ToString();
                message.SetAttribute("id", T);
                message.SetAttribute("name", T);
                start.AppendChild(message);
                XmlElement say = doc.CreateElement("say");

                message.AppendChild(say);
                say.InnerText = SpeechSentence.ToString();
               
               
              
            }
           
          string xmlString = doc.OuterXml;
                doc.Save(@getDirectoryName[0]+"//A.xml");
    
        
        }
        private void GenerateXMLFileToB(ArrayList alBMPR)
        {
            int count = 0;
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(dec);
            XmlElement start = doc.CreateElement("start");
            doc.AppendChild(start);
            for (int v = 0; v < alBMPR.Count; v++)
            {
                count++;
                string SpeechSentence = alBMPR[v].ToString();
                // string[] SentenceInfo = SpeechSentence.Split(":".ToCharArray());
                XmlElement message = doc.CreateElement("message");


                string T = "B" + count.ToString();
                message.SetAttribute("id", T);
                message.SetAttribute("name", T);
                start.AppendChild(message);
                XmlElement say = doc.CreateElement("say");

                message.AppendChild(say);
                say.InnerText = SpeechSentence.ToString();



            }

            string xmlString = doc.OuterXml;
            doc.Save(@getDirectoryName[0] + "//B.xml");


        }
        private void insertToMPR(ArrayList alAMPR)
        {
            int count = 0;
            StringBuilder sb = new StringBuilder();

            XmlDocument doc = new XmlDocument();

            string fullPath = "C:\\Users\\Qinyu\\Desktop\\New folder (3)\\Lesson7\\ActivityMedia\\lesson7A.mpr";
            doc.Load(@fullPath);

            XmlNode xn = doc.SelectSingleNode("project");

            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode xNote in xnl)
            {
                if (xNote.HasChildNodes == true)
                {

                    XmlNodeList xnl2 = xNote.ChildNodes;
                    foreach (XmlNode xNote2 in xnl2)
                    {
                        if (xNote2.HasChildNodes == true && xNote2.Name.ToString() == "scene")
                        {
                            string getNoteName = xNote2.LastChild.Name.ToString();
                            string getNoteID = xNote2.LastChild.Attributes["id"].Value.ToString();
                            //xNote2.LastChild.Attributes.n

                            for (int v = 0; v < alAMPR.Count; v++)
                            {

                                string SpeechSentence = alAMPR[v].ToString();
                                string[] SentenceInfo= SpeechSentence.Split(":".ToCharArray());
                               // if (SentenceInfo[0] == "Jordan")
                                if (SentenceInfo[0] == "Cristina")
                                    
                                {
                                    count++;
                                    XmlNodeList xnl3 = xNote2.ChildNodes;
                                    XmlElement message = doc.CreateElement("message");
                                    XmlElement say = doc.CreateElement("say");
                                    xNote2.AppendChild(message);
                                 //   string T="B"+ count.ToString();
                                    string T = "A" + count.ToString();
                                    message.SetAttribute("id",T );
                                    message.SetAttribute("name", T);
                                  
                                    message.AppendChild(say);
                                    say.InnerText = SentenceInfo[1].ToString();
                                
                                
                                }
                              


                            }

                         





                        }

                    }
                }
            }
            getSpeechSentence.Clear();
            doc.Save(@fullPath);


        }
        private string parseXML(string filename, string fullPath)
        {
            
            
            
                StringBuilder sb = new StringBuilder();

                XmlDocument doc = new XmlDocument();
                doc.Load(@fullPath);

                XmlNode xn = doc.SelectSingleNode("AutoTutorScript");

                XmlNodeList xnl = xn.ChildNodes;

                foreach (XmlNode xn1 in xnl)
                {
                    if (xn1.Name.ToString() == "Agents" || xn1.Name.ToString() == "RigidPacks" || xn1.Name.ToString() == "TutoringPacks")
                    {
                        XmlNodeList xnl2 = xn1.ChildNodes;


                        foreach (XmlNode xn2 in xnl2)
                        {
                            XmlNodeList xnl3 = xn2.ChildNodes;

                            if (xn2.Name.ToString() == "Agent")
                            {
                                XmlElement getAgent = (XmlElement)xn2;
                                string agent = getAgent.GetAttribute("name").ToString();
                                foreach (XmlNode xn3 in xnl3)
                                {
                                    if (xn3.Name.ToString() == "SpeechCan")
                                    {
                                        XmlNodeList xnl4 = xn3.ChildNodes;
                                        foreach (XmlNode xn4 in xnl4)
                                        {
                                            if (xn4.Name.ToString() == "Item")
                                            {
                                                XmlElement xe = (XmlElement)xn4;
                                                string text = xe.GetAttribute("text").ToString();
                                                string speech = xe.GetAttribute("speech").ToString();
                                                bool sentence = getSentence(speech);
                                                if (sentence == true)
                                                {
                                                   
                                                    sb.AppendLine(agent + ":" + speech);

                                                }
                                                else
                                                {

                                                    sentence = getSentence(text);
                                                    if (sentence == true)
                                                    {
                                                       
                                                        sb.AppendLine(agent + ":" + text);
                                                    }
                                                }

                                            }
                                        }


                                    }

                                }
                            }
                            else if (xn2.Name.ToString() == "RigidPack")
                            {
                                XmlNodeList xnl5 = xn2.ChildNodes;
                                foreach (XmlNode xn5 in xnl5)
                                {
                                    XmlElement xe = (XmlElement)xn5;
                                    string agent = xe.GetAttribute("agent").ToString();
                                    string text = xe.GetAttribute("text").ToString();
                                    string speech = xe.GetAttribute("speech").ToString();
                                    bool sentence = getSentence(speech);
                                    if (sentence == true)
                                    {

                                        sb.AppendLine(agent + ":" + speech);

                                    }
                                    else
                                    {

                                        sentence = getSentence(text);
                                        if (sentence == true)
                                        {
                                            sb.AppendLine(agent + ":" + text);
                                        }
                                    }
                                }


                            }
                            else if (xn2.Name.ToString() == "TutoringPack")
                            {
                                XmlNodeList TP1 = xn2.ChildNodes;
                                foreach (XmlNode TPnote1 in TP1)
                                {
                                    if (TPnote1.Name.ToString() == "Questions")
                                    {
                                        XmlNodeList TP2 = TPnote1.ChildNodes;
                                        {
                                            foreach (XmlNode TPnote2 in TP2)
                                            {
                                                if (TPnote2.Name.ToString() == "Question")
                                                {
                                                    XmlElement xe = (XmlElement)TPnote2;
                                                    string agent = xe.GetAttribute("agent").ToString();
                                                    string text = xe.GetAttribute("text").ToString();
                                                    string speech = xe.GetAttribute("speech").ToString();
                                                    bool sentence = getSentence(speech);
                                                    if (sentence == true)
                                                    {
                                                        sb.AppendLine(agent + ":" + speech);

                                                    }
                                                    else
                                                    {

                                                        sentence = getSentence(text);
                                                        if (sentence == true)
                                                        {
                                                            sb.AppendLine(agent + ":" + text);
                                                        }
                                                    }

                                                    if (TPnote2.HasChildNodes == true)
                                                    {
                                                        XmlNodeList TP3 = TPnote2.ChildNodes;
                                                        foreach (XmlNode TPnote3 in TP3)
                                                        {
                                                            if (TPnote3.HasChildNodes == true && TPnote3.Name.ToString() == "Answers")
                                                            {
                                                                XmlNodeList TP4 = TPnote3.ChildNodes;
                                                                foreach (XmlNode TPnote4 in TP4)
                                                                {

                                                                    XmlElement xe1 = (XmlElement)TPnote4;
                                                                    agent = xe.GetAttribute("agent").ToString();
                                                                    text = xe1.GetAttribute("text").ToString();
                                                                    speech = xe1.GetAttribute("speech").ToString();
                                                                    sentence = getSentence(speech);
                                                                    if (sentence == true)
                                                                    {

                                                                        sb.AppendLine(agent + ":" + speech);

                                                                    }
                                                                    else
                                                                    {

                                                                        sentence = getSentence(text);
                                                                        if (sentence == true)
                                                                        {
                                                                            sb.AppendLine(agent + ":" + text);
                                                                        }
                                                                    }
                                                                }

                                                            }


                                                        }

                                                    }
                                                }

                                            }

                                        }
                                    }


                                    else if (TPnote1.Name.ToString() == "Expectations")
                                    {
                                        XmlNodeList ex1 = TPnote1.ChildNodes;
                                        foreach (XmlNode exnote1 in ex1)
                                        {
                                            if (exnote1.HasChildNodes == true)
                                            {
                                                if (exnote1.Name.ToString() == "Expectation")
                                                {
                                                    XmlNodeList ex2 = exnote1.ChildNodes;
                                                    foreach (XmlNode exnote2 in ex2)
                                                    {
                                                        if (exnote2.HasChildNodes == true)
                                                        {
                                                            if (exnote2.Name.ToString() == "Answers" || exnote2.Name.ToString() == "Hints" || exnote2.Name.ToString() == "Prompts")
                                                            {


                                                                XmlNodeList ex3 = exnote2.ChildNodes;
                                                                foreach (XmlNode exnote3 in ex3)
                                                                {
                                                                    if (exnote3.Name.ToString() == "Answer" || exnote3.Name.ToString() == "Hint" || exnote3.Name.ToString() == "Prompt")
                                                                    {
                                                                        XmlElement xe1 = (XmlElement)exnote3;
                                                                        string agent = xe1.GetAttribute("agent").ToString();
                                                                        string text = xe1.GetAttribute("text").ToString();
                                                                        string speech = xe1.GetAttribute("speech").ToString();
                                                                        bool sentence = getSentence(speech);
                                                                        if (sentence == true)
                                                                        {
                                                                            sb.AppendLine(agent + ":" + speech);

                                                                        }
                                                                        else
                                                                        {

                                                                            sentence = getSentence(text);
                                                                            if (sentence == true)
                                                                            {
                                                                                sb.AppendLine(agent + ":" + text);
                                                                            }
                                                                        }

                                                                        if (exnote3.HasChildNodes == true)
                                                                        {

                                                                            XmlNodeList ex4 = exnote3.ChildNodes;
                                                                            foreach (XmlNode exnote4 in ex4)
                                                                            {
                                                                                XmlNodeList ex5 = exnote4.ChildNodes;
                                                                                XmlElement xe2 = (XmlElement)exnote4;
                                                                                agent = xe2.GetAttribute("agent").ToString();
                                                                                text = xe2.GetAttribute("text").ToString();
                                                                                speech = xe2.GetAttribute("speech").ToString();
                                                                                sentence = getSentence(speech);
                                                                                if (sentence == true)
                                                                                {
                                                                                    sb.AppendLine(agent + ":" + speech);

                                                                                }
                                                                                else
                                                                                {

                                                                                    sentence = getSentence(text);
                                                                                    if (sentence == true)
                                                                                    {
                                                                                        sb.AppendLine(agent + ":" + text);
                                                                                    }
                                                                                }

                                                                                if (exnote4.HasChildNodes == true)
                                                                                {
                                                                                    foreach (XmlNode exnote5 in ex5)
                                                                                    {

                                                                                        XmlElement xe3 = (XmlElement)exnote5;
                                                                                        agent = xe2.GetAttribute("agent").ToString();
                                                                                        text = xe3.GetAttribute("text").ToString();
                                                                                        speech = xe3.GetAttribute("speech").ToString();
                                                                                        sentence = getSentence(speech);
                                                                                        if (sentence == true)
                                                                                        {
                                                                                            sb.AppendLine(agent + ":" + speech);

                                                                                        }
                                                                                        else
                                                                                        {

                                                                                            sentence = getSentence(text);
                                                                                            if (sentence == true)
                                                                                            {
                                                                                                sb.AppendLine(agent + ":" + text);
                                                                                            }
                                                                                        }

                                                                                    }

                                                                                }




                                                                            }


                                                                        }

                                                                    }
                                                                }


                                                            }
                                                        }

                                                    }

                                                }

                                            }
                                        }
                                    }


                                }

                            }

                        }

                    }
                }


                return sb.ToString();

        }

      

        private void button3_Click(object sender, EventArgs e)
        {
           
            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(parseXML);
            Thread myThread = new Thread(ParStart);
            object o = "saveSentences";
            myThread.Start(o);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(parseXML);
            Thread myThread = new Thread(ParStart);
            object o = "insertToMPR";
            myThread.Start(o);


        }

    
       


    }
}
