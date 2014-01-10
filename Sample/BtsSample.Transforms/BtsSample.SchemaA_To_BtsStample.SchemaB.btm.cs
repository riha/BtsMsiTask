namespace BtsSample.Transforms {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BtsSample.Schemas.SchemaA", typeof(global::BtsSample.Schemas.SchemaA))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"BtsSample.Schemas.SchemaB", typeof(global::BtsSample.Schemas.SchemaB))]
    public sealed class BtsSample_SchemaA_To_BtsStample_SchemaB : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var s0"" version=""1.0"" xmlns:ns0=""http://BtsSample.Schemas.SchemaB"" xmlns:s0=""http://BtsSample.Schemas.SchemaA"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/s0:SchemaARoot"" />
  </xsl:template>
  <xsl:template match=""/s0:SchemaARoot"" />
</xsl:stylesheet>";
        
        private const string _strArgList = @"<ExtensionObjects />";
        
        private const string _strSrcSchemasList0 = @"BtsSample.Schemas.SchemaA";
        
        private const global::BtsSample.Schemas.SchemaA _srcSchemaTypeReference0 = null;
        
        private const string _strTrgSchemasList0 = @"BtsSample.Schemas.SchemaB";
        
        private const global::BtsSample.Schemas.SchemaB _trgSchemaTypeReference0 = null;
        
        public override string XmlContent {
            get {
                return _strMap;
            }
        }
        
        public override string XsltArgumentListContent {
            get {
                return _strArgList;
            }
        }
        
        public override string[] SourceSchemas {
            get {
                string[] _SrcSchemas = new string [1];
                _SrcSchemas[0] = @"BtsSample.Schemas.SchemaA";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"BtsSample.Schemas.SchemaB";
                return _TrgSchemas;
            }
        }
    }
}
