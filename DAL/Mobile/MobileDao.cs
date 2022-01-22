using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.Model.Mobile;
using static ZHONGJIAN_API.Common.MySqlDBHelper;

namespace ZHONGJIAN_API.DAL
{
    public class MobileDao
    {
        #region 项目库
        /// <summary>
        /// 项目列表
        /// </summary>
        /// <returns></returns>
        public DataTable SelectProjectList(ProjectListDto param, out int count)
        {
            string where = " ";
            List<MySqlParameter> listParam = new List<MySqlParameter>();
            #region  查询搜索条件
            if (param.filter != null)
            {
                var filter = param.filter;
                //地区公司（可多选，选择内容以逗号隔开  如：“安徽公司,江苏公司”）
                if (!string.IsNullOrWhiteSpace(filter.company_name))
                {
                    //where += " and a.company_name in(?company_name) ";
                    //listParam.Add(new MySqlParameter("?company_name", companyname));
                    //string[] companyNamearr = filter.company_name.Split(',');
                    where += $" and  FIND_IN_SET(a.company_name,'{string.Join(',', filter.company_name)}') > 0";
                }
                //项目模式（可多选，选择内容以逗号隔开  如：“BOT,产业园投资”）
                if (!string.IsNullOrWhiteSpace(filter.project_mode))
                {
                    //where += " and a.project_mode in(?project_mode) ";
                    //listParam.Add(new MySqlParameter("?project_mode", projectmode));
                    //string[] project_modearr = filter.project_mode.Split(',');
                    where += $" and  FIND_IN_SET(a.project_mode,'{string.Join(',', filter.project_mode)}') > 0";
                }
                //项目名称
                if (!string.IsNullOrWhiteSpace(filter.project_name))
                {
                    where += " and a.project_name like ?project_name ";
                    listParam.Add(new MySqlParameter("?project_name", "%" + filter.project_name.Trim() + "%"));
                }
            }
            #endregion 
            string sql = string.Format(@"SELECT  a.Id,a.company_name,a.projectCategory,a.project_mode,a.project_name,a.stage_status,a.summary,a.{1} 
            from eform_edrms_project_manage a where 1=1 {0} order by {1} {2} limit {3},{4};", where, param.dynField, param.dynOrder, param.page, param.rows);
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, listParam.ToArray());
            string countSql = string.Format(@"SELECT COUNT(1) FROM eform_edrms_project_manage a where 1=1 {0};", where);
            count = Convert.ToInt32(new MySqlDBHelper(ConfigHelper.Conn).ExecuteScalar(countSql, listParam.ToArray()));
            DataTable dt = ds.Tables[0];
            dt.DefaultView.Sort = param.dynField + " " + param.dynOrder;
            dt = dt.DefaultView.ToTable();
            return dt;
        }

        /// <summary>
        /// 根据项目ID获取项目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetProjectInfoById(string projectId)
        {
            string sql = $"select t.Id,t.project_name,t.company_name,t.Province,t.city,t.ProvinceCity,t.amount,t.equity_ratio,t.cooperation_period,t.cooperation_level,t.project_mode,t.stage_status,t.project_type,t.projectCategory,t.meeting_date,t.win_bid_date,t.project_person,phone,t.default_img_url,t.img_url,t.summary from eform_edrms_project_manage t where Id = @projectId";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@projectId",projectId),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }
        #endregion

        #region 档案库

        /// <summary>
        /// 根据档案ID和档案表获取档案信息
        /// </summary>
        /// <param name="arch_table_name"></param>
        /// <param name="archId"></param>
        /// <returns></returns>
        public DataTable GetArchInfoById(string arch_table_name, string archId)
        {
            string sql = $"select t.Id,t.archtypeid,t.mainCompanyName,t.mainProjectName,t.projectName,t.mainProjectMode,t.mainProjectType,t.`name`,t.archType,t.writtendate,t.edition,t.note,t.folderId,t.refileFolderId from eform_edrms_{arch_table_name} t where t.Id=@archId";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@archId",archId),
            };

            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        /// <summary>
        /// 根据档案ID获取附件（电子文件）
        /// </summary>
        /// <param name="archId"></param>
        /// <returns></returns>
        public DataTable GetAttachInfoByArchId(string archId)
        {
            string sql = $"SELECT t.Id,t.ControlId,t.RefID,t.FileId,t.`Name`,t.FileSize,t.VersionId,t.Version,t.NewestVersion,t.FullPath,t.ParentFolderId,b.user_name Creator,t.CreateTime,b.user_name ModifyOperator,t.ModifiedTime,t.Remark FROM eform_attachment  t,org_user a, org_user b WHERE t.Creator = a.user_identityID AND t.ModifyOperator = b.user_id and RefID=@archId";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@archId",archId),
            };

            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        #endregion

        #region 流程

        /// <summary>
        /// 流程列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataTable SelectFlowList(FlowListDto param, out int count)
        {
            var procName = "Proc_MyInbox";
            if (param.proType == 0)//我的待办 -存储过程:Proc_MyInbox
            {
                procName = "Proc_MyInbox";
            }
            else if (param.proType == 1)//在途流程（我的已办）-存储过程:Proc_MyComplete
            {
                procName = "Proc_MyComplete";
            }
            else if (param.proType == 2)//完结流程（我的归档）-存储过程:Proc_MyArchive
            {
                procName = "Proc_MyArchive";
            }

            #region 参数
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@userId",param.userId),
                new MySqlParameter("@processDefinitionKey",""),
                new MySqlParameter("@processDefinitionName",""),
                new MySqlParameter("@starter",""),
                new MySqlParameter("@processCategory",""),
                new MySqlParameter("@processInstanceId",""),
                new MySqlParameter("@summary",""),
                new MySqlParameter("@totalRecord",0),
                new MySqlParameter("@pageIndex",param.page),
                new MySqlParameter("@pageSize",param.rows),
                };
            #endregion
            count = 0;
            DataTable dt = new MySqlDBHelper(ConfigHelper.Conn).GetDataByProcedureName(procName, out count, mysqlParams);

            return dt;
        }

        /// <summary>
        /// 根据recordId查询项目库档案归档流程档案列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public DataTable GetArchPlaceByRecordId(string recordId)
        {
            string sql = $"select t.Id,t.archTypeId,t.detailsId,t.duration,t.`name`,t.entryType,t.recordId,u.user_account,u.user_id,u.user_name from eform_edrms_archplacedetails t,org_user u where t.createId = u.user_id and t.recordId = @recordId;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@recordId",recordId),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }
        /// <summary>
        /// 根据recordId查询档案借阅流程档案列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public DataTable GetBorrowInfoByRecordId(string recordId)
        {
            string sql = $"SELECT t.Id,t.archTypeId,t.BorrowMethod,t.archTypeName,t.`name`,t.archiveId,t.number,t.brrowid,t.borrower,t.borrowStatus,t.brrowday,t.brrowe,t.brrownum,t.brrowpurpose,t.brrowreason,t.brrows,t.formId,t.isWatermark,t.mainCompanyName,t.mainProjectName,t.returnTime,t.sectName from eform_edrms_borrowarchinfo t where t.brrowid = @recordId;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@recordId",recordId),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }
        /// <summary>
        /// 根据Id查询档案借阅流程信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetBorrowarchInfoById(string id)
        {
            string sql = $"SELECT t.Id,t.`name`,t.date,t.dept,t.brrowreason,t.managerName,t.archuserName,t.assistanterName,t.supervisorName,t.manager,t.archuser,t.assistanter,t.supervisor,t.startUser,t.borrowDeptId from eform_edrms_borrow t where t.Id = @id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@id",id),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        /// <summary>
        /// 根据流程ID（包含版本）查询流程所有变量
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public DataTable GetProcessVars(string processId)
        {
            string sql = $"SELECT ev.* FROM ext_design_def edd LEFT JOIN ext_processmanage ep ON edd.key_ = ep.key_ AND edd.REV_ = ep.REV_ LEFT JOIN ext_var ev ON ep.id_ = ev.PROCESSMANAGE_ID_ WHERE edd.act_def_id_ = @processId;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@processId",processId),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        #region 档案第三方
        /// <summary>
        /// 更新流程的第三方 thirdTodoId
        /// </summary>
        /// <param name="archplaceId"></param>
        /// <param name="thirdTodoId"></param>
        /// <returns></returns>
        public int UpdateThirdTodoIdArchplaceById(string archplaceId, string thirdTodoId)
        {
            string sql = $" update eform_edrms_archplace set thirdTodoId =@thirdTodoId  where Id = @archplaceId;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@archplaceId",archplaceId),
                new MySqlParameter("@thirdTodoId",thirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }

        /// <summary>
        /// 修改归档发起人流程的第三方id
        /// </summary>
        /// <param name="archplaceId"></param>
        /// <param name="startUserthirdTodoId"></param>
        /// <returns></returns>
        public int UpdateStartUserThirdTodoIdArchplaceById(string archplaceId, string startUserthirdTodoId)
        {
            string sql = $" update eform_edrms_archplace set startUserthirdTodoId =@startUserthirdTodoId  where Id = @archplaceId;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@archplaceId",archplaceId),
                new MySqlParameter("@startUserthirdTodoId",startUserthirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        } 
        #endregion

        #region 借阅第三方
        /// <summary>
        /// 修改借阅流程的负责人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="managerThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateManagerThirdTodoIdBorrowById(string Id, string managerThirdTodoId)
        {
            string sql = $" update eform_edrms_borrow set managerThirdTodoId =@managerThirdTodoId  where Id = @Id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@Id",Id),
                new MySqlParameter("@managerThirdTodoId",managerThirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }
        /// <summary>
        /// 修改借阅流程的档案第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="archuserThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateArchuserThirdTodoIdBorrowById(string Id, string archuserThirdTodoId)
        {
            string sql = $" update eform_edrms_borrow set archuserThirdTodoId =@archuserThirdTodoId  where Id = @Id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@Id",Id),
                new MySqlParameter("@archuserThirdTodoId",archuserThirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }
        /// <summary>
        /// 修改借阅流程的总部区域对接人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="assistanterThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateAssistanterThirdTodoIdBorrowById(string Id, string assistanterThirdTodoId)
        {
            string sql = $" update eform_edrms_borrow set assistanterThirdTodoId =@assistanterThirdTodoId  where Id = @Id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@Id",Id),
                new MySqlParameter("@assistanterThirdTodoId",assistanterThirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }
        /// <summary>
        /// 修改借阅流程的总部第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="supervisorThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateSupervisorThirdTodoIdBorrowById(string Id, string supervisorThirdTodoId)
        {
            string sql = $" update eform_edrms_borrow set supervisorThirdTodoId =@supervisorThirdTodoId  where Id = @Id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@Id",Id),
                new MySqlParameter("@supervisorThirdTodoId",supervisorThirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }
        /// <summary>
        /// 修改借阅流程的加签人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="plusThirdTodoId"></param>
        /// <returns></returns>
        public int UpdatePlusThirdTodoIdBorrowById(string Id, string plusThirdTodoId)
        {
            string sql = $" update eform_edrms_borrow set plusThirdTodoId =@plusThirdTodoId  where Id = @Id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@Id",Id),
                new MySqlParameter("@plusThirdTodoId",plusThirdTodoId),
            };
            return new MySqlDBHelper(ConfigHelper.Conn).ExecuteNonQuery(sql, mysqlParams);
        }

        #endregion

        #endregion

    }
}
