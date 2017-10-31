using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using log4net;
using Lpp.Objects.Dynamic;
using Lpp.QueryComposer;

namespace Lpp.Dns.DataMart.Model.QueryComposer
{
    public class QueryComposerRequestInterrogator
    {
        static readonly ILog _logger = LogManager.GetLogger(typeof(QueryComposerRequestInterrogator));
        readonly DTO.QueryComposer.QueryComposerRequestDTO _request;
        readonly IEnumerable<DTO.QueryComposer.QueryComposerFieldDTO> _selectFields;        
        readonly DTO.QueryComposer.QueryComposerCriteriaDTO _primaryCriteria = null;
        DTO.QueryComposer.QueryComposerTermDTO _primaryObservationPeriodTerm = null;
        
        public QueryComposerRequestInterrogator(DTO.QueryComposer.QueryComposerRequestDTO request)
        {
            _request = request;
            if (_request.Select == null)
            {
                _request.Select = new DTO.QueryComposer.QueryComposerSelectDTO
                {
                    Fields = Enumerable.Empty<DTO.QueryComposer.QueryComposerFieldDTO>()
                };
            }

            if (_request.Select.Fields == null)
            {
                _request.Select.Fields = Enumerable.Empty<DTO.QueryComposer.QueryComposerFieldDTO>();
            }

            _selectFields = _request.Select.Fields;

            if (_request.Where.Criteria == null)
                _request.Where.Criteria = Enumerable.Empty<DTO.QueryComposer.QueryComposerCriteriaDTO>();

            _primaryCriteria = _request.Where.Criteria.FirstOrDefault();
            if (_primaryCriteria != null)
            {
                _primaryObservationPeriodTerm = _primaryCriteria.Terms.FirstOrDefault(t => t.Type == ModelTermsFactory.ObservationPeriodID);
            }
        }

        public bool HasCriteria
        {
            get
            {
                return _request.Where != null && 
                       _request.Where.Criteria != null && 
                       _request.Where.Criteria.Any() &&
                       _request.Where.Criteria.SelectMany(c => c.Criteria.SelectMany(cc => cc.Terms)).Concat(_request.Where.Criteria.SelectMany(c => c.Terms)).Any();
            }
        }

        public bool IsSQLDistribution
        {
            get {
                if (!HasCriteria)
                    return false;

                return _request.Where.Criteria.SelectMany(c => c.Criteria.SelectMany(cc => cc.Terms).Where(t => t.Type == ModelTermsFactory.SqlDistributionID)).Concat(_request.Where.Criteria.SelectMany(c => c.Terms).Where(t => t.Type == ModelTermsFactory.SqlDistributionID)).Any();
            }
        }

        public bool HasStratifiers
        {
            get
            {
                return _selectFields.Any(f => f.StratifyBy != null);
            }
        }

        public bool HasAggregates
        {
            get
            {
                return _selectFields.Any(f => f.Aggregate != null);
            }
        }

        public bool IsSimpleAggregate
        {
            get
            {
                return _selectFields.Count() == 1 && _selectFields.Any(f => f.Aggregate != null);
            }
        }

        public bool NeedsToGroup
        {
            get
            {
                //grouping occurs when the select contains fields that have aggregation and some don't.
                return _selectFields.Count() > 1 && _selectFields.Any(f => f.Aggregate == null) && _selectFields.Any(f => f.Aggregate != null);
            }
        }

        public DTO.QueryComposer.QueryComposerTermDTO PrimaryObservationPeriodTerm
        {
            get
            {
                return _primaryObservationPeriodTerm;
            }
        }

        public Lpp.Dns.DataMart.Model.QueryComposer.Adapters.DateRangeValues PrimaryObservationPeriodDateRange
        {
            get
            {
                if (_primaryObservationPeriodTerm != null)
                {
                    var range = Lpp.Dns.DataMart.Model.QueryComposer.Adapters.AdapterHelpers.ParseDateRangeValues(_primaryObservationPeriodTerm);
                    if (range.StartDate.HasValue || range.EndDate.HasValue)
                    {
                        return range;
                    }
                }

                return null;
            }
        }
        
    }
}
