using System;
using System.Data.SqlClient;

namespace Harbour.DBUtilsStatic
{
    /// <summary>
    /// 事物回滚类型
    /// </summary>
    public enum EffentNextType
    {
        /// <summary>
        /// 对其他语句无任何影响 
        /// </summary>
        None,
        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果存在则继续执行，不存在回滚事务
        /// </summary>
        WhenHaveContine,
        /// <summary>
        /// 当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        WhenNoHaveContine,
        /// <summary>
        /// 当前语句影响到的行数必须大于0，否则回滚事务
        /// </summary>
        ExcuteEffectRows,
        /// <summary>
        /// 引发事件-当前语句必须为"select count(1) from .."格式，如果不存在则继续执行，存在回滚事务
        /// </summary>
        SolicitationEvent
    }   
    
    /// <summary>
    /// CommandInfo 对象信息
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// 共享对象
        /// </summary>
        public object ShareObject = null;

        /// <summary>
        /// 原始对象
        /// </summary>
        public object OriginalData = null;
        
        event EventHandler _solicitationEvent;
        
        /// <summary>
        /// 表示将处理不包含事件数据的事件的方法
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
        /// 表示将处理不包含事件数据的事件的方法
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
        /// 参数列表
        /// </summary>
        public System.Data.Common.DbParameter[] Parameters;
        /// <summary>
        /// 事物回滚类型
        /// </summary>
        public EffentNextType EffentNextType = EffentNextType.None;

        /// <summary>
        /// 初始化
        /// </summary>
        public CommandInfo()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sqlText">SQL</param>
        /// <param name="para">参数</param>
        public CommandInfo(string sqlText, SqlParameter[] para)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sqlText">SQL</param>
        /// <param name="para">参数</param>
        /// <param name="type">类型</param>
        public CommandInfo(string sqlText, SqlParameter[] para, EffentNextType type)
        {
            this.CommandText = sqlText;
            this.Parameters = para;
            this.EffentNextType = type;
        }
    }
}
