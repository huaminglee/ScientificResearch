//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18449
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.ServiceModel.DomainServices.Intellisense {
    
    
    [System.AttributeUsageAttribute(System.AttributeTargets.All, AllowMultiple=true)]
    public sealed class SourceInfoAttribute : System.Attribute {
        
        private string _fileName;
        
        private int _line;
        
        private int _column;
        
        public string FileName {
            get {
                return this._fileName;
            }
            set {
                this._fileName = value;
            }
        }
        
        public int Line {
            get {
                return this._line;
            }
            set {
                this._line = value;
            }
        }
        
        public int Column {
            get {
                return this._column;
            }
            set {
                this._column = value;
            }
        }
    }
}
