using Lpp.CNDS.DTO;
using Lpp.Dns.DTO;
using Lpp.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lpp.CNDS.ApiClient.Helpers
{
    public class DomainData
    {
        public static DomainDataDTO CreateDomainData(Guid domainUseID, Guid? domainReferenceID, string value = null, int sequeceNumber = 0)
        {
            var domainData = new DomainDataDTO() {
                DomainUseID = domainUseID,
                DomainReferenceID = domainReferenceID,
                SequenceNumber = sequeceNumber,
            };
            if (!value.IsNullOrWhiteSpace())
                domainData.Value = value;

            return domainData;
        }

        public static DomainDataDTO UpdateDomainData(DomainDataDTO current, string value = null, int sequeceNumber = 0)
        {
            current.Value = value;
            current.SequenceNumber = sequeceNumber;
            return current;
        }

        public static IEnumerable<DomainDataDTO> GetReferenceItems(MetadataDTO editDTO, IEnumerable<DomainDataDTO> currentMetaData)
        {
            List<DTO.DomainDataDTO> metaData = new List<DTO.DomainDataDTO>();

            if (!editDTO.IsMultiValue && !editDTO.Value.IsNullOrWhiteSpace())
            {
                var current = currentMetaData.Where(cm => cm.DomainUseID == editDTO.DomainUseID).FirstOrDefault();
                if (current != null && current.DomainReferenceID == Guid.Parse(editDTO.Value))
                {
                    metaData.Add(UpdateDomainData(current, null, 0));
                }
                else
                {
                    metaData.Add(CreateDomainData(editDTO.DomainUseID.Value, Guid.Parse(editDTO.Value), null, 0));
                }

            }
            else if (editDTO.IsMultiValue)
            {
                foreach (var child in editDTO.References)
                {
                    if (child.Value != null && child.Value != "" && !string.Equals(child.Value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        if (currentMetaData.Any(cm => cm.DomainUseID == editDTO.DomainUseID && cm.DomainReferenceID == child.ID))
                        {
                            string newVal = null;
                            if (!string.Equals(child.Value, "true", StringComparison.OrdinalIgnoreCase))
                                newVal = child.Value;
                            metaData.Add(UpdateDomainData(currentMetaData.Where(cm => cm.DomainUseID == editDTO.DomainUseID && cm.DomainReferenceID == child.ID).FirstOrDefault(), newVal, 0));
                        }
                        else
                        {
                            string newVal = null;
                            if (!string.Equals(child.Value, "true", StringComparison.OrdinalIgnoreCase))
                                newVal = child.Value;
                            metaData.Add(CreateDomainData(editDTO.DomainUseID.Value, child.ID, newVal, 0));
                        }
                    }
                }

            }


            return metaData;
        }


        public static IEnumerable<DomainDataDTO> GetMetdataChildren(MetadataDTO editDTO, IEnumerable<DomainDataDTO> currentMetaData)
        {
            List<DTO.DomainDataDTO> metaData = new List<DTO.DomainDataDTO>();

            foreach (var meta in editDTO.ChildMetadata)
            {
                if (meta.DataType != "reference" && meta.DataType != "boolean")
                {
                    if (meta.Value != null && meta.Value != "")
                    {
                        if (currentMetaData.Any(cm => cm.DomainUseID == meta.DomainUseID))
                        {
                            metaData.Add(UpdateDomainData(currentMetaData.Where(cm => cm.DomainUseID == meta.DomainUseID).FirstOrDefault(), meta.Value, 0));
                        }
                        else
                        {
                            metaData.Add(CreateDomainData(meta.DomainUseID.Value, null, meta.Value, 0));
                        }
                    }
                    if (meta.ChildMetadata != null && meta.ChildMetadata.Count() > 0)
                        metaData.AddRange(GetMetdataChildren(meta, currentMetaData));
                }
                else if (meta.DataType == "boolean")
                {
                    if (meta.Value == "true")
                    {
                        if (currentMetaData.Any(cm => cm.DomainUseID == meta.DomainUseID))
                        {
                            metaData.Add(UpdateDomainData(currentMetaData.Where(cm => cm.DomainUseID == meta.DomainUseID).FirstOrDefault(), null, 0));
                        }
                        else
                        {
                            metaData.Add(CreateDomainData(meta.DomainUseID.Value, null, null, 0));
                        }
                    }
                }
                else
                {

                    if (meta.References != null && meta.References.Count() > 0)
                        metaData.AddRange(GetReferenceItems(meta, currentMetaData));

                    if (meta.ChildMetadata != null && meta.ChildMetadata.Count() > 0)
                        metaData.AddRange(GetMetdataChildren(meta, currentMetaData));
                }

            }



            return metaData;
        }

        public static IEnumerable<DomainDataDTO> GetDomainData(IEnumerable<MetadataDTO> editDTO, IEnumerable<DomainDataDTO> currentMetaData)
        {
            List<DTO.DomainDataDTO> metaData = new List<DTO.DomainDataDTO>();

            foreach (var meta in editDTO)
            {
                if (meta.DataType != "reference" && meta.DataType != "boolean")
                {
                    if (meta.Value != null && meta.Value != "")
                    {
                        if (currentMetaData.Any(cm => cm.DomainUseID == meta.DomainUseID))
                        {
                            metaData.Add(UpdateDomainData(currentMetaData.Where(cm => cm.DomainUseID == meta.DomainUseID).FirstOrDefault(), meta.Value, 0));
                        }
                        else
                        {
                            metaData.Add(CreateDomainData(meta.DomainUseID.Value, null, meta.Value, 0));
                        }
                    }
                    if (meta.ChildMetadata != null && meta.ChildMetadata.Count() > 0)
                        metaData.AddRange(GetMetdataChildren(meta, currentMetaData));
                }
                else if(meta.DataType == "boolean")
                {
                    if(meta.Value == "true")
                    {
                        if (currentMetaData.Any(cm => cm.DomainUseID == meta.DomainUseID))
                        {
                            metaData.Add(UpdateDomainData(currentMetaData.Where(cm => cm.DomainUseID == meta.DomainUseID).FirstOrDefault(), null, 0));
                        }
                        else
                        {
                            metaData.Add(CreateDomainData(meta.DomainUseID.Value, null, null, 0));
                        }
                    }
                }
                else
                {

                    if (meta.References != null && meta.References.Count() > 0)
                        metaData.AddRange(GetReferenceItems(meta, currentMetaData));

                    if (meta.ChildMetadata != null && meta.ChildMetadata.Count() > 0)
                        metaData.AddRange(GetMetdataChildren(meta, currentMetaData));                  
                }
            }



            return metaData;
        }
    }
}
