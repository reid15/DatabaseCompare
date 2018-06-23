using System.Collections.Generic;

namespace DatabaseCompare
{
    public enum JobTypeEnum
    {
        SchemaCompare,
        DataCompare
    }

    public class JobTypeEntry
    {
        public JobTypeEnum JobTypeId { get; set; }
        public string JobTypeName { get; set; }

        public JobTypeEntry(
            JobTypeEnum jobTypeId,
            string jobTypeName
        )
        {
            JobTypeId = jobTypeId;
            JobTypeName = jobTypeName;
        }
        public override string ToString()
        {
            return JobTypeName;
        }
    }

    public class JobType
    {
        public static List<JobTypeEntry> GetJobTypeList()
        {
            var returnList = new List<JobTypeEntry>();
            returnList.Add(new JobTypeEntry(JobTypeEnum.SchemaCompare, "Schema Compare"));
            returnList.Add(new JobTypeEntry(JobTypeEnum.DataCompare, "Data Compare"));

            return returnList;
        }

    }
}
