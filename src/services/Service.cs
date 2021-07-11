namespace ArletBank
{
    public abstract class Service<T> where T : new() {
        public Service(Model<T> model)
        {
            Model = model;
        }
        
        protected Model<T> Model { get; set; }
    } 
}