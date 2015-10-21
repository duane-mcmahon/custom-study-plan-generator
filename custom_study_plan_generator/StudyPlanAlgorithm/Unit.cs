using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace custom_study_plan_generator.StudyPlanAlgorithm
{
    public class Unit
    {

        private string unitName;
        private string unitCode;
        private string unitType;
        private string semester;
        private string year;
        private string preReq = null;
        private bool isPreReq;
        private bool exempt;
        public Unit(string _unitName, string _unitCode, string _unitType, string _semester, string _year, string _preReq, bool _isPreReq, bool _exempt)
        {
            unitName = _unitName;
            unitCode = _unitCode;
            unitType = _unitType;
            semester = _semester;
            year = _year;
            preReq = _preReq;
            isPreReq = _isPreReq;
            exempt = _exempt;
        }

        public string UnitName
        {
            get
            {
                return unitName;
            }
            set
            {
                unitName = value;
            }
        }
        public string UnitCode
        {
            get
            {
                return unitCode;
            }
            set
            {
                unitCode = value;
            }
        }
        public string UnitType
        {
            get
            {
                return unitType;
            }
            set
            {
                unitType = value;
            }
        }
        public string Semester
        {
            get
            {
                return semester;
            }
            set
            {
                semester = value;
            }
        }
        public string Year
        {
            get
            {
                return year;
            }
            set
            {
                year = value;
            }
        }
        public string PreReq
        {
            get
            {
                return preReq;
            }
            set
            {
                preReq = value;
            }
        }
        public bool IsPreReq
        {
            get
            {
                return isPreReq;
            }
            set
            {
                isPreReq = value;
            }
        }
        public bool Exempt
        {
            get
            {
                return exempt;
            }
            set
            {
                exempt = value;
            }
        }
    }
}