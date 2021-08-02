namespace ArletBank
{
    /// <summary>
    /// A service is a collection of methods on a model.
    /// </summary>
    /// <typeparam name="T">The type this model serves as a wrapper for</typeram>
    public abstract class Service<T> where T : new() {
        public Service(Model<T> model)
        {
            Model = model;
        }

        protected Model<T> Model { get; set; }
    } 
}