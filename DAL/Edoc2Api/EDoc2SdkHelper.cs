using EDoc2.IAppService;
using EDoc2.IAppService.BusinessModel.Permission;
using EDoc2.IAppService.Model;
using EDoc2.IAppService.Model.Organization;
using EDoc2.IAppService.ResultModel;
using EDoc2.Sdk;
using EDoc2.Sdk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.Core.Edoc2Api;
using ZHONGJIAN_API.DAL.Edoc2Api;

namespace ZHONGJIAN_API.Edoc2Api
{
    public class EDoc2SdkHelper
    {
        private static readonly IOrgAppService _edoc2OrgApi;
        private static readonly IFolderAppService _edoc2FolderApi;
        private static readonly IFolderPermissionAppService _edoc2FolderPermissionApi;
        private static readonly IFileAppService _edoc2FileApi;
        private static readonly IDocAppService _edoc2DocApi;
        private static readonly IFileAttachAppService _edoc2FileAttachApi;
        private static readonly IMetaDataAppService _edoc2MetaDataApi;
        private static readonly ITransferAppService _transferApi;
        private static readonly IOrgDepartmentAppService _orgDepartmentApi;
        private static readonly IOrgUserAppService _orgUserApi;
        static EDoc2SdkHelper()
        {
            //忽略[System.Security.Authentication.AuthenticationException：根据验证过程，远程证书无效。]
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            SdkBaseInfo.BaseUrl = ConfigHelper.Edoc2BaseUrl;
            _edoc2OrgApi = ServiceContainer.GetService<IOrgAppService>();
            _edoc2FolderApi = ServiceContainer.GetService<IFolderAppService>();
            _edoc2FolderPermissionApi = ServiceContainer.GetService<IFolderPermissionAppService>();
            _edoc2FileApi = ServiceContainer.GetService<IFileAppService>();
            _edoc2DocApi = ServiceContainer.GetService<IDocAppService>();
            _edoc2FileAttachApi = ServiceContainer.GetService<IFileAttachAppService>();
            _edoc2MetaDataApi = ServiceContainer.GetService<IMetaDataAppService>();
            _transferApi = ServiceContainer.GetService<ITransferAppService>();
            _orgDepartmentApi = ServiceContainer.GetService<IOrgDepartmentAppService>();
            _orgUserApi = ServiceContainer.GetService<IOrgUserAppService>();
        }

        public static string GetToken(string userName = null)
        {
            if (string.IsNullOrEmpty(userName))
            {
                userName = ConfigHelper.Edoc2SdkLoginName;
            }
            UserLoginIntegrationByUserLoginNameDto dto = new UserLoginIntegrationByUserLoginNameDto();
            dto.IPAddress = "127.0.0.1";
            dto.IntegrationKey = ConfigHelper.Edoc2SdkIntegrationKey;
            dto.LoginName = userName;
            var result = _edoc2OrgApi.UserLoginIntegrationByUserLoginName(dto);
            if (result.Result != 0)
            {
                throw new Exception($"UserLoginIntegrationByUserLoginName erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
            }
            return result.Data;
        }

        ///// <summary>
        ///// 集成登录
        ///// </summary>
        //public static BaseResponse<string> UserLoginIntegrationByUserLoginName(string loginName = null)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(loginName))
        //        {
        //            loginName = ConfigHelper.Edoc2SdkLoginName;
        //        }
        //        UserLoginIntegrationByUserLoginNameDto dto = new UserLoginIntegrationByUserLoginNameDto();
        //        dto.IPAddress = "127.0.0.1";
        //        dto.IntegrationKey = ConfigHelper.Edoc2SdkIntegrationKey;
        //        dto.LoginName = loginName;
        //        var sdkResult = _edoc2OrgApi.UserLoginIntegrationByUserLoginName(dto);
        //        return BaseResponseHelper.BiudTstringFromSdkResult(sdkResult);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// 集成登录
        /// </summary>
        public static string UserLogin(string loginName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(loginName))
                {
                    loginName = ConfigHelper.Edoc2SdkLoginName;
                }
                LoginDto dto = new LoginDto();
                dto.IpAddress = "127.0.0.1";
                dto.Password = null;
                dto.UserName = loginName;
                var sdkResult = _edoc2OrgApi.UserLoginByNoPassword(dto);
                if (string.IsNullOrEmpty(sdkResult.UserToken))
                {
                    throw new Exception("UserToken is null,loginName = " + loginName ?? "null");
                }
                return sdkResult.UserToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 文件夹操作

        /// <summary>
        ///     获取文件夹名称
        /// </summary>
        public static string GetFolderNameById(int folderId)
        {
            try
            {
                //判断文件夹是否存在
                var result = _edoc2FolderApi.GetFolderInfoById(GetToken(), folderId);
                if (result.Result == 0)
                {
                    return result.Data.FolderName;
                }
                throw new Exception($"r={result.Result},desc={result.DataDescription},msg={result.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("GetFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        ///     获取文件夹id(如果文件夹不存在则新建文件夹并返回新建文件夹的id)
        /// </summary>
        /// <param name="parentFolderId">所在文件夹id</param>
        /// <param name="folderName">文件夹名称</param>
        /// <returns>文件夹id</returns>
        public static int GetFolder(int parentFolderId, string folderName,string foldercode="")
        {
            try
            {
                //判断文件夹是否存在
                //var result = _edoc2FolderApi.IsExistfolderInFolderByfolderName(GetToken(), folderName, parentFolderId);
                var folderId = GetFolderIdInFolderByfolderName(folderName, parentFolderId);
                if (folderId > 0)
                {
                    return folderId;
                }
                return CreateFolder(parentFolderId, folderName, foldercode);
            }
            catch (Exception ex)
            {
                throw new Exception("GetFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 根据文件夹路径获取文件夹ID
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static int GetFolderInfoByNamePath(string folderNamePath)
        {
            try
            {
                string token = GetToken();
                var result = _edoc2FolderApi.GetFolderInfoByNamePath(token, folderNamePath);
                if (result.Result != 0)
                {
                    throw new Exception("CreateFolder Erro,result=" + result.Result);
                }
                return result.Data.FolderId;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 判断指定文件夹下是否存在指定名字的文件夹，并获取ID
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static int GetFolderIdInFolderByfolderName(string folderName,int folderId)
        {
            try
            {
                string token = GetToken();
                var result = _edoc2FolderApi.GetFolderIdInFolderByfolderName(token, folderName, folderId);
                if (result.Result != 0)
                {
                    throw new Exception("CreateFolder Erro,result=" + result.Result);
                }
                return result.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFolder Exception," + ex.Message);
            }
        }
        /// <summary>
        /// 判断指定文件夹下是否存在指定名字的文件夹
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool IsExistfolderInFolderByfolderName(string folderName, int folderId)
        {
            try
            {
                string token = GetToken();
                var result = _edoc2FolderApi.IsExistfolderInFolderByfolderName(token, folderName, folderId);
                if (result.Result != 0)
                {
                    throw new Exception("CreateFolder Erro,result=" + result.Result);
                }
                return result.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static void RemoveFolderList(int folderId)
        {
            try
            {
                string token = GetToken();
                FolderListDto dto = new FolderListDto
                {
                    FolderIdList = new List<int> { folderId },
                    Token = token
                };
                var result = _edoc2FolderApi.RemoveFolderList(dto);
                if (result.Result != 0)
                {
                    throw new Exception("RemoveFolderList Erro,result=" + result.Result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RemoveFolderList Exception," + ex.Message);
            }
        }

        //重命名文件夹
        public static void RenameFolder(int folderId,string newName)
        {
            
            try
            {
                string token = GetToken();
                RenameDto dto = new RenameDto
                {
                    DocId=folderId,
                    NewName= newName,
                    Token = token
                };
                var result = _edoc2FolderApi.RenameFolder(dto);
                if (result.Result != 0)
                {
                    throw new Exception("RenameFolder Erro,result=" + result.Result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("RenameFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="parentFolderId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static void CopyFolderList(int targetFolderId,int copyfolderId)
        {
            try
            {
                string token = GetToken();
                CopyFolderListDto dto = new CopyFolderListDto
                {
                    TargetFolderId = targetFolderId,
                    FolderIdList = new List<int> { copyfolderId },
                    CopyMeta = true,
                    CopyPerm = false,
                    Token = token
                };
                var result = _edoc2FolderApi.CopyFolderList(dto);
                if (result.Result != 0)
                {
                    throw new Exception("CopyFolderList Erro,result=" + result.Result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("CopyFolderList Exception," + ex.Message);
            }
        }
        //public static int GetFolderPermCates()
        //{
        //    try
        //    {
        //        string token = GetToken();
        //        var result = _edoc2DocApi.GetFolderPermCatesByEntryType(dto);
        //        if (result.Result != 0)
        //        {
        //            throw new Exception("CreateFolder Erro,result=" + result.Result);
        //        }
        //        return result.Data;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("CreateFolder Exception," + ex.Message);
        //    }
        //}

        //public static int SetPermissions(int folderId,int userId)
        //{
        //    try
        //    {
        //        string token = GetToken();
        //        SetFolderPermDto dto = new SetFolderPermDto();
        //        MemberPermission memberPermission = new MemberPermission();
        //        memberPermission.MemberId = userId;
        //        memberPermission.MemberType = 1;
        //        memberPermission.PermCateId = 
        //        dto.FolderId = folderId;
        //        dto.Token = token;
        //        //dto.Permissions = 
        //        var result = _edoc2FolderPermissionApi.SetFolderPermission(dto);
        //        if (result.Result != 0)
        //        {
        //            throw new Exception("CreateFolder Erro,result=" + result.Result);
        //        }
        //        return result.Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("CreateFolder Exception," + ex.Message);
        //    }
        //}

        /// <summary>
        ///     新建文件夹
        /// </summary>
        /// <param name="parentFolderId">所在文件夹id</param>
        /// <param name="folderName">文件夹名称</param>
        /// <returns>Result:文件夹id</returns>
        public static int CreateFolder(int parentFolderId, string folderName,string foldercode="")
        {
            try
            {
                string token = GetToken();
                var tokenUserInfo = _edoc2OrgApi.GetCurrentUserInfo(token);
                CreateFolderDto dto = new CreateFolderDto();
                dto.Name = folderName;
                dto.FolderCode = foldercode;
                dto.Remark = "";
                dto.ParentFolderId = parentFolderId;
                dto.Token = token;
                var result = _edoc2FolderApi.CreateFolder(dto);
                if (result.Result != 0)
                {
                    throw new Exception("CreateFolder Erro,result=" + result.Result);
                }
                return result.Data.FolderId;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFolder Exception," + ex.Message);
            }
        }

        /// <summary>
        ///     删除文件夹(出现异常返回1,代表未知错误)
        /// </summary>
        /// <param name="folderId">文件夹id</param>
        /// <returns>删除是否成功</returns>
        public static int DeleteFolder(int folderId, string token=null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                List<int> folderIds = new List<int> {
                    folderId
                };
                FolderListDto folderListDto = new FolderListDto();
                folderListDto.FolderIdList = folderIds;
                folderListDto.Token = token;
                var result = _edoc2FolderApi.RemoveFolderList(folderListDto);
                if (result.Result != 0)
                {
                    throw new Exception($"r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                return result.Result;
            }
            catch (Exception ex)
            {
                throw new Exception("DeleteFolder ex," + ex.Message);
            }
        }
       

        
        #endregion

        #region 文件操作

        public static Edoc2GetFileInfoByIdRes GetFileInfoById(int fileid, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                var result = _edoc2FileApi.GetFileInfoById(token, fileid);
                if (result.Result != 0)
                {
                    throw new Exception($"r={result.Result},desc={result.DataDescription},msg={result.Message}");
                }
                if (result.Data != null)
                {
                    Edoc2GetFileInfoByIdRes r = new Edoc2GetFileInfoByIdRes();
                    r.FileId = result.Data.FileId;
                    r.FileName = result.Data.FileName;
                    r.FilePath = result.Data.FilePath;
                    return r;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("GetFileIdsByFolderId ex," + ex.Message);
            }
        }

        public static IList<int> GetFileIdsByFolderId(int atFolderId, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                var result = _edoc2FileApi.GetChildFileListByFolderId(token, atFolderId);
                if (result.Result != 0)
                {
                    throw new Exception($"r={result.Result},desc={result.DataDescription},msg={result.Message}");
                }
                if (result.Data != null && result.Data.Count > 0)
                {
                    return result.Data.Select(i => i.FileId).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("GetFileIdsByFolderId ex," + ex.Message);
            }
        }

        /// <summary>
        ///     创建文件
        /// </summary>
        /// <param name="atFolderId">所在文件夹</param>
        /// <param name="fileName">文件全名称(含路径)</param>
        /// <param name="renameFileName">文件名称(自定义在edoc2中的名称)</param>
        /// <returns>result大于0为成功，result为文件id</returns>
        public static int CreateFile(int atFolderId, string fileName, string renameFileName = null)
        {
            try
            {
                string token = GetToken();
                UploadFileResult r = null;
                if (renameFileName == null)
                {
                    r = Uploader.UploadFile(token, fileName, atFolderId);
                }
                else
                {
                    r = Uploader.UploadFile(token, fileName, atFolderId, upgradeStrategy: UpgradeStrategy.Rename, renameFileName: renameFileName);
                }
                return r.File.FileId;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFile Exception," + ex.Message);
            }
        }

        /// <summary>
        ///     更新文件
        /// </summary>
        /// <param name="atFolderId">所在文件夹</param>
        /// <param name="fileName">文件全名称(含路径)</param>
        /// <param name="renameFileName">文件名称(自定义在edoc2中的名称)</param>
        /// <returns>result大于0为成功，result为文件id</returns>
        public static int UpdateFile(int fromFileId,int toFileId,int atFolderId, string fileName)
        {
            try
            {
                string token = GetToken();
                //获取文件流
                Stream stream = Edoc2DownLoadHelper.GetFileStream(fromFileId.ToString(),token);
                UploadFileResult r = null;
                r = Uploader.UpdateFile(token, toFileId, fileName, stream, atFolderId, UpdateUpgradeStrategy.MajorUpgrade,CalcMD5.None);
                return r.File.FileId;
            }
            catch (Exception ex)
            {
                throw new Exception("CreateFile Exception," + ex.Message);
            }
        }


        /// <summary>
        ///     删除文件(出现异常返回1,代表未知错误)
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <returns>删除是否成功</returns>
        public static int DeleteFile(int fileId, string token)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                List<int> fileIds = new List<int>
                {
                    fileId
                };

                EDoc2.IAppService.Model.FileListDto dto = new EDoc2.IAppService.Model.FileListDto();
                dto.Token = token;
                dto.FileIdList = fileIds;
                var result = _edoc2FileApi.RemoveFileList(dto);
                if (result.Result != 0)
                {
                    throw new Exception($"r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                return result.Result;
            }
            catch (Exception ex)
            {
                throw new Exception("DeleteFile ex," + ex.Message);
            }
        }
        /// <summary>
        /// 复制单个文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="targetFolderId"></param>
        /// <returns></returns>
        public static int CopyFile(int fileId, int targetFolderId)
        {
            try
            {
                EDoc2.IAppService.Model.CopySingleFileDto dto = new CopySingleFileDto();
                dto.Token = GetToken();
                dto.TargetFolderId = targetFolderId;
                dto.FileId = fileId;
                dto.CopyMeta = false;
                dto.CopyPerm = false;
                dto.CopyCustomEvents = false;

                var result = _edoc2DocApi.CopySingleFile(dto);
                if (result.Result == 0)
                {
                    return result.Data.FileId;
                }
                throw new Exception("CopyFile erro,result = " + result.Result);
            }
            catch (Exception ex)
            {
                throw new Exception("CopyFile Exception," + ex.Message);
            }
        }
        /// <summary>
        /// 复制文件列表
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="targetFolderId"></param>
        /// <returns></returns>
        public static List<int> CopyFileList(List<int>fileIds,int targetFolderId)
        {
            try
            {
                CopyFileListDto dto = new CopyFileListDto();

                dto.Token = GetToken();
                dto.TargetFolderId = targetFolderId;
                dto.FileIdList = fileIds;
                dto.CopyMeta = true;
                dto.CopyPerm = false;
                dto.CopyCustomEvents = false;

                var result = _edoc2DocApi.CopyFileListReturnId(dto);
                if (result.Result == 0)
                {
                    return result.Data.ToList();
                }
                throw new Exception("CopyFile erro,result = " + result.Result);
            }
            catch (Exception ex)
            {
                throw new Exception("CopyFile Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="fileIdList"></param>
        /// <param name="targetFolderId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int MoveFiles(List<int> fileIdList, int targetFolderId, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }

                MoveFileListAndFolderListDto dto = new MoveFileListAndFolderListDto();
                dto.FileIdList = fileIdList;
                dto.FolderIdList = new List<int>();
                dto.TargetFolderId = targetFolderId;
                dto.Token = token;
                var result = _edoc2DocApi.MoveFolderListAndFileList(dto);
                //if (result.Result != 0)
                //{
                //    throw new Exception($"MoveFiles erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                //}
                return 0;
            }
            catch (Exception ex)
            {
                //throw new Exception("MoveFiles Exception," + ex.Message);
                return 0;
            }
        }
        #endregion

        /// <summary>
        /// 根据文件id获取文件的绝对路径
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static FileInfoForSdkResult GetFullFileInfoById(int fileId)
        {
            try
            {
                var token = GetToken();
                var result = _edoc2FileApi.GetFileInfoById(token, fileId);
                if (result.Result != 0)
                {
                    throw new Exception($"GetFiles erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }

                return result.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("GetFiles Exception," + ex.Message);
            }
        }

        public static DepartmentInfo GetDepartmentInfoById(string deptId)
        {
            try
            {
                var token = GetToken();
                var result = _orgDepartmentApi.GetDepartmentInfoById(token, deptId);
                if (result.Result == 0 && result.Data != null)
                {
                    return result.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static UserInfo GetUserInfoByAccount(string account)
        {
            try
            {
                var token = GetToken();
                var result = _orgUserApi.GetUserInfoByAccount(token, account);
                if (result.Result == 0 && result.Data != null)
                {
                    return result.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region 附件
        /// <summary>
        ///  给某个文件创建附件文件列表
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="attachFileIdList"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int CreateAttachFileList(int fileId, List<int> attachFileIdList, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                FileAttachFileDto dto = new FileAttachFileDto();
                dto.FileId = fileId;
                dto.AttachFileIdList = attachFileIdList;
                dto.Token = token;
                var result = _edoc2FileAttachApi.CreateAttachFileList(dto);
                if (result.Result != 0)
                {
                    //throw new Exception($"CreateAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                    LogHelper.Info($"CreateAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.Info("CreateAttachFileList Exception," + ex.Message);
                return 0;
                //throw new Exception("CreateAttachFileList Exception," + ex.Message);
            }
        }

        public static List<int> GetAttachFileList(int fileId, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                var result = _edoc2FileAttachApi.GetAttachFileList(token,fileId,1,100);
                if (result.Result != 0)
                {
                    LogHelper.Info($"GetAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                    //throw new Exception($"GetAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                if (result.Data.AttachFileList.Count() > 0)
                {
                    return result.Data.AttachFileList.Select(i => i.FileId).ToList();
                }
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.Info("CreateAttachFileList Exception," + ex.Message);
                return null;
                //throw new Exception("CreateAttachFileList Exception," + ex.Message);
            }
        }

        public static int RemoveAttachFileList(int fileId, List<int> attachFileIdList, string token = null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                FileAttachFileDto dto = new FileAttachFileDto();
                dto.FileId = fileId;
                dto.AttachFileIdList = attachFileIdList;
                dto.Token = token;
                var result = _edoc2FileAttachApi.RemoveAttachFileList(dto);
                if (result.Result != 0)
                {
                    LogHelper.Info($"RemoveAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                    return 0;
                    //throw new Exception($"RemoveAttachFileList erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.Info("CreateAttachFileList Exception," + ex.Message);
                return 0;
                //throw new Exception("CreateAttachFileList Exception," + ex.Message);
            }
        }
        #endregion

        #region 元数据

        public static FileMetaResult GetMetaDatasByFileId(string fileId,string docType,string token=null)
        {
            try
            {
                if (token == null)
                {
                    token = GetToken();
                }
                var result = _edoc2MetaDataApi.GetMetaDatasByFileId(token,fileId,docType);
                if (result.Result != 0)
                {
                    throw new Exception($"GetMetaDatasByFileId erro,r={result.Result},d={result.Data},desc={result.DataDescription},msg={result.Message}");
                }
                if (result.Data.Count > 0)
                {
                    return result.Data[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("GetMetaDatasByFileId Exception," + ex.Message);
            }
        }

        /// <summary>
        /// 单个文件下载，同步
        /// </summary>
        public static FileStream Download(int fileId,string token=null)
        {
            return Downloader.DownloadSingleFile(token, fileId, "d:\\test.txt");
        }
        #endregion


        public static bool CheckUserTokenValidity(string token)
        {
            try
            {
                var result = _edoc2OrgApi.CheckUserTokenValidity(token);
                if (result.Result == 0 && result.Data)
                {
                    return result.Data;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static UserInfo GetUserInfoByToken(string token)
        {
            try
            {
                var result = _orgUserApi.GetUserInfoByToken(token);
                if (result.Result == 0 && result.Data!= null)
                {
                    return result.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static UserInfo GetUserInfoByUserId(string token ,string userId)
        {
            try
            {
                var result = _orgUserApi.GetUserInfoByUserGuid(token,userId);
                if (result.Result == 0 && result.Data != null)
                {
                    return result.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
