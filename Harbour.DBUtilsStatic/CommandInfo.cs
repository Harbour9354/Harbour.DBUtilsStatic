using System;
using System.Data.SqlClient;

namespace Harbour.DBUtilsStatic
{
    /// <summary>
    /// ����ع�����
    /// </summary>
    public enum EffentNextType
    {
        /// <summary>
        /// ������������κ�Ӱ�� 
        /// </summary>
        None,
        /// <summary>
        /// ��ǰ������Ϊ"select count(1) from .."��ʽ��������������ִ�У������ڻع�����
        /// </summary>
        WhenHaveContine,
        /// <summary>
        /// ��ǰ������Ϊ"select count(1) from .."��ʽ����������������ִ�У����ڻع�����
        /// </summary>
        WhenNoHaveContine,
        /// <summary>
        /// ��ǰ���Ӱ�쵽�������������0������ع�����
        /// </summary>
        ExcuteEffectRows,
        /// <summary>
        /// �����¼�-��ǰ������Ϊ"select count(1) from .."��ʽ����������������ִ�У����ڻع�����
        /// </summary>
        SolicitationEvent
    }   
    
    /// <summary>
    /// CommandInfo ������Ϣ
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// �������
        /// </summary>
        public object ShareObject = null;

        /// <summary>
        /// ԭʼ����
        /// </summary>
        public object OriginalData = null;
        
        event EventHandler _solicitationEvent;
        
        /// <summary>
        /// ��ʾ�����������¼����ݵ��¼��ķ���
        /// </summary>
        public event EventHandler SolicitationEvent
        {
            add
            {
                _solicitationEvent += value;
            }
            remove
            {
                _solicitationEvent -= value;
            }
        }

        /// <summary>
        /// ��ʾ�����������¼����ݵ��¼��ķ���
        /// </summary>
        public void OnSolicitationEvent()
        {
            if (_solicitationEvent != null)
            {
                _solicitationEvent(this,new EventArgs());
            }
        }
        
        /// <summary>
        /// SQL
        /// </summary>
        public string CommandText;
        /// <summary>
        /// �����б�
        /// </summary>
        public System.Data.Common.DbParameter[] Parameters;
        /// <summary>
        /// ����ع�����
        /// </summary>
        public EffentNextType EffentNextType = EffentNextType.None;

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public CommandInfo()
        {

        }
        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="sqlText">SQL</param>
        /// <param name="para">����</param>
        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
        }
        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="sqlText">SQL</param>
        /// <param name="para">����</param>
        /// <param name="type">����</param>
        public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
            this.EffentNextType = type;
        }
    }
}
