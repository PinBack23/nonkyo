using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

namespace NOnkyo.WpfGui.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        #region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            if (!this.CheckExpressionForMemberAccess(propertyExpression.Body))
                throw new ArgumentException("propertyExpression",
                        string.Format("The expected expression is no 'MemberAccess'; its a '{0}'", propertyExpression.Body.NodeType));


            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyNameFromExpression(propertyExpression)));
        }

        private bool CheckExpressionForMemberAccess(System.Linq.Expressions.Expression propertyExpression)
        {
            return propertyExpression.NodeType == ExpressionType.MemberAccess;
        }

        public string GetPropertyNameFromExpression<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)propertyExpression.Body;

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            else
                throw new ArgumentException("propertyExpression");
        }

        #endregion

        #region Attributes

        private Boolean? mbIsDialogClose;

        #endregion

        #region Constructor / Destructor

        public ViewModelBase()
        {
            this.ErrorList = new Dictionary<string, string>();
        }

        #endregion

        #region Public Methods / Properties

        public Dictionary<string, string> ErrorList { get; private set; }

        public Boolean? IsDialogClose
        {
            get { return mbIsDialogClose; }
            set
            {
                mbIsDialogClose = value;
                this.OnPropertyChanged(() => this.IsDialogClose);
            }
        }

        #endregion

        #region IDataErrorInfo Member

        public virtual string Error
        {
            get { return string.Empty; }
        }

        public virtual string this[string columnName]
        {
            get
            {
                if (this.ErrorList.ContainsKey(columnName))
                    return this.ErrorList[columnName];
                return string.Empty;
            }
        }

        #endregion
    }
}
