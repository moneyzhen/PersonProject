using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class ArchModel
    {
        public List<entryDataEsQuery> entryDataEsQueryList { get; set; }
        public int totalCount { get; set; }
    }
    public class entryDataEsQuery
    {
        public string Id { get; set; }
        public string archType { get; set; }
        public string archTypeChange { get; set; }
        public int archiveObjectType { get; set; }
        public string archivedate { get; set; }
        public string archiver { get; set; }
        public string archivername { get; set; }
        public string archtypeid { get; set; }
        public string archtypename { get; set; }
        public string carrier { get; set; }
        public string createId { get; set; }
        public string createTime { get; set; }
        public string deadTime { get; set; }
        public string duration { get; set; }
        public string edition { get; set; }
        public string entrystate { get; set; }
        public List<files> files { get; set; }
        public string firstChar { get; set; }
        public string folderId { get; set; }
        public string formid { get; set; }
        public string id { get; set; }
        public string id_path { get; set; }
        public string ifDossiered { get; set; }
        public string ifInbound { get; set; }
        public string isDossier { get; set; }
        public string is_flag { get; set; }
        public string is_show { get; set; }
        public string lastAutoAddNo { get; set; }
        public string littleStatus { get; set; }
        public string mainAmount { get; set; }
        public string mainCompanyName { get; set; }
        public string mainDataNo { get; set; }
        public string mainProjectCode { get; set; }
        public string mainProjectMode { get; set; }
        public string mainProjectName { get; set; }
        public string mainProjectType { get; set; }
        public string modifiedTime { get; set; }
        public string month { get; set; }
        public string name { get; set; }
        public string note { get; set; }
        public string number { get; set; }
        public string objectid { get; set; }
        public string objtype { get; set; }
        public string pa_status { get; set; }
        public string project_manage_id { get; set; }
        public string refileFolderId { get; set; }
        public string reorganizedate { get; set; }
        public string reorganizer { get; set; }
        public string reorganizername { get; set; }
        public string secert { get; set; }
        public string secertduration { get; set; }
        public string sectid { get; set; }
        public string sectname { get; set; }
        public string stage_status { get; set; }
        public string tableName { get; set; }
        public string times { get; set; }
        public string updateId { get; set; }
        public string writtendate { get; set; }
        public int year { get; set; }
    }

    public class files {
        public int fileid { get; set; }
    }





}
