using compareWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace EPocalipse.Json.JsonView
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            //ֻ����һ��ʵ��
            Process instance = RunningInstance();
            if (instance == null)
            {
                //1.1 û��ʵ��������
                Application.Run(new MainForm());
               // Application.Run(new CompareWindow());
            }
            else
            {
                //1.2 �Ѿ���һ��ʵ��������
                HandleRunningInstance(instance);
            }
        }


        //2.�ڽ����в����Ƿ��Ѿ���ʵ��������
        #region  ȷ������ֻ����һ��ʵ��
        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //�����뵱ǰ����������ͬ�Ľ����б� 
            foreach (Process process in processes)
            {
                //���ʵ���Ѿ���������Ե�ǰ���� 
                if (process.Id != current.Id)
                {
                    //��֤Ҫ�򿪵Ľ���ͬ�Ѿ����ڵĽ�������ͬһ�ļ�·��
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        //�����Ѿ����ڵĽ���
                        return process;
                    }
                }
            }
            return null;
        }
        //3.�Ѿ����˾Ͱ�����������䴰�ڷ�����ǰ��
        private static void HandleRunningInstance(Process instance)
        {
            //MessageBox.Show("�Ѿ������У�", "��ʾ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //PostMessage(instance.MainWindowHandle, 0x0407, 100, 0); 
            MessageBox.Show("�Ѿ���ʵ�������У��벻Ҫ�ظ�����");
        }

        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);  
        
        #endregion
 
    }
        
}