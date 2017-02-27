namespace StudentManagementSystem.Data.ViewModels
{
    using System.Linq;
    using System.Web.Mvc;

    public class JsonReturnViewModel<T>
        where T : class
    {
        public JsonReturnViewModel(T element)
        {
            this.element = element;
        }

        public JsonReturnViewModel(T element, ModelStateDictionary modelstate)
        {
            this.element = element;
            this.haserror = !modelstate.IsValid;
            this.errormessage = this.GetStateError(modelstate);
        }

        public string GetStateError(ModelStateDictionary modelstate)
        {
            if (!modelstate.IsValid)
            {
                return string.Join(".", modelstate.SelectMany(s => s.Value.Errors).Select(s => s.ErrorMessage));
            }

            return string.Empty;
        }

        public T element { get; set; }

        public bool haserror { get; set; }

        public string errormessage { get; set; }
    }
}
