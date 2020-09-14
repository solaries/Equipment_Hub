using System;
using System.Collections.Generic;
using System.Linq;
using NPoco;

namespace Equipment_hub.Data.Models 
{
	public partial class Equipment_hub : Database
	{
		public Equipment_hub() : base("Equipment_hub")
		{
			CommonConstruct();
		}
		public virtual void CommonConstruct()
		{
		    Factory = new DefaultFactory();
		}
		public interface IFactory
		{
			Equipment_hub GetInstance();
		    void BeginTransaction(Equipment_hub database);
		    void CompleteTransaction(Equipment_hub database);
		}


        public class DefaultFactory : IFactory
        {
            [ThreadStatic]
            static Stack<Equipment_hub> _stack = new Stack<Equipment_hub>();

            public Equipment_hub GetInstance()
            {
               
			    if (_stack == null)
                { return new  Equipment_hub(); }
                else { 
					return _stack.Count > 0 ? _stack.Peek() : new Equipment_hub();
                }
			   
			    
            }

            public void BeginTransaction(Equipment_hub database)
            {

			 if (_stack == null)
				 {
				  _stack = new  Stack<Equipment_hub>();
				 }
                _stack.Push(database);
            }

            public void CompleteTransaction(Equipment_hub database)
            {
			 if (_stack == null)
				 {
				  _stack = new Stack <Equipment_hub>();
				 }
                _stack.Pop();
            }
        }
		
		public static IFactory Factory { get; set; }

        public static Equipment_hub GetInstance()
        {
		 if (Factory == null)
                return new Equipment_hub();
			return Factory.GetInstance();
        }

		protected override void OnBeginTransaction()
		{
            Factory.BeginTransaction(this);
		}

        protected override void OnCompleteTransaction()
		{
            Factory.CompleteTransaction(this);
		}

		public class Record<T> where T:new()
		{
			public bool IsNew(Database db) { return db.IsNew(this); }
			public object Insert(Database db) { return db.Insert(this); }  
			
			public int Update(Database db, IEnumerable<string> columns) { return db.Update(this, columns); }
			public static int Update(Database db, string sql, params object[] args) { return db.Update<T>(sql, args); }
			public static int Update(Database db, Sql sql) { return db.Update<T>(sql); }
			public int Delete(Database db) { return db.Delete(this); }
			public static int Delete(Database db, string sql, params object[] args) { return db.Delete<T>(sql, args); }
			public static int Delete(Database db, Sql sql) { return db.Delete<T>(sql); }
			public static int Delete(Database db, object primaryKey) { return db.Delete<T>(primaryKey); }
			public static bool Exists(Database db, object primaryKey) { return db.Exists<T>(primaryKey); }
			public static T SingleOrDefault(Database db, string sql, params object[] args) { return db.SingleOrDefault<T>(sql, args); }
			public static T SingleOrDefault(Database db, Sql sql) { return db.SingleOrDefault<T>(sql); }
			public static T FirstOrDefault(Database db, string sql, params object[] args) { return db.FirstOrDefault<T>(sql, args); }
			public static T FirstOrDefault(Database db, Sql sql) { return db.FirstOrDefault<T>(sql); }
			public static T Single(Database db, string sql, params object[] args) { return db.Single<T>(sql, args); }
			public static T Single(Database db, Sql sql) { return db.Single<T>(sql); }
			public static T First(Database db, string sql, params object[] args) { return db.First<T>(sql, args); }
			public static T First(Database db, Sql sql) { return db.First<T>(sql); }
			public static List<T> Fetch(Database db, string sql, params object[] args) { return db.Fetch<T>(sql, args); }
			public static List<T> Fetch(Database db, Sql sql) { return db.Fetch<T>(sql); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Fetch<T>(page, itemsPerPage, sql, args); }
			public static List<T> Fetch(Database db, long page, long itemsPerPage, Sql sql) { return db.Fetch<T>(page, itemsPerPage, sql); }
			public static List<T> SkipTake(Database db, long skip, long take, string sql, params object[] args) { return db.SkipTake<T>(skip, take, sql, args); }
			public static List<T> SkipTake(Database db, long skip, long take, Sql sql) { return db.SkipTake<T>(skip, take, sql); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, string sql, params object[] args) { return db.Page<T>(page, itemsPerPage, sql, args); }
			public static Page<T> Page(Database db, long page, long itemsPerPage, Sql sql) { return db.Page<T>(page, itemsPerPage, sql); }
			public static IEnumerable<T> Query(Database db, string sql, params object[] args) { return db.Query<T>(sql, args); }
			public static IEnumerable<T> Query(Database db, Sql sql) { return db.Query<T>(sql); }			
			
			protected HashSet<string> Tracker = new HashSet<string>();
			private void OnLoaded() { Tracker.Clear(); }
			protected void Track(string c) { if (!Tracker.Contains(c)) Tracker.Add(c); }

			public int Update(Database db) 
			{ 
				if (Tracker.Count == 0)
					return db.Update(this); 

				var retv = db.Update(this, Tracker.ToArray());
				Tracker.Clear();
				return retv;
			}
			public void Save(Database db) 
			{
                if (this.IsNew(db))
					Insert(db);
				else
					Update(db);
			}		
		}	
	}
	 [TableName("Equipment_hub_event")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class @Event : Equipment_hub.Record<@Event>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventName")] public string Eventname 
			{ 
				get { return _Eventname; }
				set { _Eventname = value; Track("eventName"); }
			}
			string _Eventname;
		
			public static IEnumerable<@Event> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("Equipment_hub_event (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<@Event>(sql);
            }

		} [TableName("Equipment_hub_eventLog")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class EventLog : Equipment_hub.Record<EventLog>

		{
			[Column("id")] public long Id 
			{ 
				get { return _Id; }
				set { _Id = value; Track("id"); }
			}
			long _Id;
			[Column("eventid")] public long Eventid 
			{ 
				get { return _Eventid; }
				set { _Eventid = value; Track("eventid"); }
			}
			long _Eventid;
			[Column("description")] public string Description 
			{ 
				get { return _Description; }
				set { _Description = value; Track("description"); }
			}
			string _Description;
			[Column("userEvent")] public bool Userevent 
			{ 
				get { return _Userevent; }
				set { _Userevent = value; Track("userEvent"); }
			}
			bool _Userevent;
			[Column("userid")] public long Userid 
			{ 
				get { return _Userid; }
				set { _Userid = value; Track("userid"); }
			}
			long _Userid;
			[Column("eventDate")] public DateTime Eventdate 
			{ 
				get { return _Eventdate; }
				set { _Eventdate = value; Track("eventDate"); }
			}
			DateTime _Eventdate;
		
			public static IEnumerable<EventLog> Query(Database db, string[] columns = null, long[] Id = null)
            {
                var sql = new Sql();

                if (columns != null)
                    sql.Select(columns);

                sql.From("Equipment_hub_eventLog (NOLOCK)");


				if (Id != null)
					sql.Where("id IN (@0)", Id);


                return db.Query<EventLog>(sql);
            }

		} [TableName("Equipment_hub_Activity_Status")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Activity_Status : Equipment_hub.Record<Equipment_hub_Activity_Status>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("activity_status_name")]  
     public  string   Activity_status_name  
     {  
         get { return _Activity_status_name; }  
         set { _Activity_status_name = value; Track("activity_status_name"); }  
      }  
      string   _Activity_status_name;  
    
 public static IEnumerable<Equipment_hub_Activity_Status> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Activity_Status (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Activity_Status>(sql);
     }
  }


 [TableName("Equipment_hub_authenticate_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_authenticate_Admin : Equipment_hub.Record<Equipment_hub_authenticate_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
 public static IEnumerable<Equipment_hub_authenticate_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_authenticate_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_authenticate_Admin>(sql);
     }
  }


 [TableName("Equipment_hub_authenticate_Customer")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_authenticate_Customer : Equipment_hub.Record<Equipment_hub_authenticate_Customer>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("activated")]  
     public   Int16   Activated  
     {  
         get { return _Activated; }  
         set { _Activated = value; Track("activated"); }  
      }  
       Int16   _Activated;  
    
     [Column("company_name")]  
     public  long  Company_name  
     {  
         get { return _Company_name; }  
         set { _Company_name = value; Track("company_name"); }  
      }  
      long  _Company_name;  
    
 public static IEnumerable<Equipment_hub_authenticate_Customer> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_authenticate_Customer (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_authenticate_Customer>(sql);
     }
  }


 [TableName("Equipment_hub_authenticate_Vendor")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_authenticate_Vendor : Equipment_hub.Record<Equipment_hub_authenticate_Vendor>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("first_name")]  
     public  string   First_name  
     {  
         get { return _First_name; }  
         set { _First_name = value; Track("first_name"); }  
      }  
      string   _First_name;  
    
     [Column("last_name")]  
     public  string   Last_name  
     {  
         get { return _Last_name; }  
         set { _Last_name = value; Track("last_name"); }  
      }  
      string   _Last_name;  
    
     [Column("email")]  
     public  string   Email  
     {  
         get { return _Email; }  
         set { _Email = value; Track("email"); }  
      }  
      string   _Email;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("password")]  
     public  byte[]  Password  
     {  
         get { return _Password; }  
         set { _Password = value; Track("password"); }  
      }  
      byte[]  _Password;  
    
     [Column("password2")]  
     public  byte[]  Password2  
     {  
         get { return _Password2; }  
         set { _Password2 = value; Track("password2"); }  
      }  
      byte[]  _Password2;  
    
     [Column("activated")]  
     public   Int16   Activated  
     {  
         get { return _Activated; }  
         set { _Activated = value; Track("activated"); }  
      }  
       Int16   _Activated;  
    
     [Column("company_name")]  
     public  long  Company_name  
     {  
         get { return _Company_name; }  
         set { _Company_name = value; Track("company_name"); }  
      }  
      long  _Company_name;  
    
 public static IEnumerable<Equipment_hub_authenticate_Vendor> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_authenticate_Vendor (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_authenticate_Vendor>(sql);
     }
  }


 [TableName("Equipment_hub_Customer_Company")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Customer_Company : Equipment_hub.Record<Equipment_hub_Customer_Company>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("company_name")]  
     public  string   Company_name  
     {  
         get { return _Company_name; }  
         set { _Company_name = value; Track("company_name"); }  
      }  
      string   _Company_name;  
    
 public static IEnumerable<Equipment_hub_Customer_Company> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Customer_Company (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Customer_Company>(sql);
     }
  }


 [TableName("Equipment_hub_Duration_Type")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Duration_Type : Equipment_hub.Record<Equipment_hub_Duration_Type>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("duration_name")]  
     public  string   Duration_name  
     {  
         get { return _Duration_name; }  
         set { _Duration_name = value; Track("duration_name"); }  
      }  
      string   _Duration_name;  
    
 public static IEnumerable<Equipment_hub_Duration_Type> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Duration_Type (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Duration_Type>(sql);
     }
  }


 [TableName("Equipment_hub_Equipment")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Equipment : Equipment_hub.Record<Equipment_hub_Equipment>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("equipment_type")]  
     public  long  Equipment_type  
     {  
         get { return _Equipment_type; }  
         set { _Equipment_type = value; Track("equipment_type"); }  
      }  
      long  _Equipment_type;  
    
     [Column("equipment_code")]  
     public  string   Equipment_code  
     {  
         get { return _Equipment_code; }  
         set { _Equipment_code = value; Track("equipment_code"); }  
      }  
      string   _Equipment_code;  
    
     [Column("vendor")]  
     public  long  Vendor  
     {  
         get { return _Vendor; }  
         set { _Vendor = value; Track("vendor"); }  
      }  
      long  _Vendor;  
    
 public static IEnumerable<Equipment_hub_Equipment> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Equipment (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Equipment>(sql);
     }
  }


 [TableName("Equipment_hub_Equipment_Activity")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Equipment_Activity : Equipment_hub.Record<Equipment_hub_Equipment_Activity>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("equipment_type")]  
     public  long  Equipment_type  
     {  
         get { return _Equipment_type; }  
         set { _Equipment_type = value; Track("equipment_type"); }  
      }  
      long  _Equipment_type;  
    
     [Column("number_of_units")]  
     public   Int16   Number_of_units  
     {  
         get { return _Number_of_units; }  
         set { _Number_of_units = value; Track("number_of_units"); }  
      }  
       Int16   _Number_of_units;  
    
     [Column("rent_rate")]  
     public  long  Rent_rate  
     {  
         get { return _Rent_rate; }  
         set { _Rent_rate = value; Track("rent_rate"); }  
      }  
      long  _Rent_rate;  
    
     [Column("rent_rate_qunatity")]  
     public   Int16   Rent_rate_qunatity  
     {  
         get { return _Rent_rate_qunatity; }  
         set { _Rent_rate_qunatity = value; Track("rent_rate_qunatity"); }  
      }  
       Int16   _Rent_rate_qunatity;  
    
     [Column("customer_company")]  
     public  long  Customer_company  
     {  
         get { return _Customer_company; }  
         set { _Customer_company = value; Track("customer_company"); }  
      }  
      long  _Customer_company;  
    
     [Column("vendor_company")]  
     public  long  Vendor_company  
     {  
         get { return _Vendor_company; }  
         set { _Vendor_company = value; Track("vendor_company"); }  
      }  
      long  _Vendor_company;  
    
     [Column("activity_status")]  
     public  long  Activity_status  
     {  
         get { return _Activity_status; }  
         set { _Activity_status = value; Track("activity_status"); }  
      }  
      long  _Activity_status;  
    
     [Column("activity_date")]  
     public  DateTime  Activity_date  
     {  
         get { return _Activity_date; }  
         set { _Activity_date = value; Track("activity_date"); }  
      }  
      DateTime  _Activity_date;  
    
 public static IEnumerable<Equipment_hub_Equipment_Activity> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Equipment_Activity (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Equipment_Activity>(sql);
     }
  }


 [TableName("Equipment_hub_Equipment_Type")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Equipment_Type : Equipment_hub.Record<Equipment_hub_Equipment_Type>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("name_of_equipment")]  
     public  string   Name_of_equipment  
     {  
         get { return _Name_of_equipment; }  
         set { _Name_of_equipment = value; Track("name_of_equipment"); }  
      }  
      string   _Name_of_equipment;  
    
 public static IEnumerable<Equipment_hub_Equipment_Type> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Equipment_Type (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Equipment_Type>(sql);
     }
  }


 [TableName("Equipment_hub_Rent_Rate")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Rent_Rate : Equipment_hub.Record<Equipment_hub_Rent_Rate>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("equipement_type")]  
     public  long  Equipement_type  
     {  
         get { return _Equipement_type; }  
         set { _Equipement_type = value; Track("equipement_type"); }  
      }  
      long  _Equipement_type;  
    
     [Column("duration_type")]  
     public  long  Duration_type  
     {  
         get { return _Duration_type; }  
         set { _Duration_type = value; Track("duration_type"); }  
      }  
      long  _Duration_type;  
    
     [Column("qunatity")]  
     public   Int16   Qunatity  
     {  
         get { return _Qunatity; }  
         set { _Qunatity = value; Track("qunatity"); }  
      }  
       Int16   _Qunatity;  
    
     [Column("price")]  
     public   float   Price  
     {  
         get { return _Price; }  
         set { _Price = value; Track("price"); }  
      }  
       float   _Price;  
    
     [Column("entry_date")]  
     public  DateTime  Entry_date  
     {  
         get { return _Entry_date; }  
         set { _Entry_date = value; Track("entry_date"); }  
      }  
      DateTime  _Entry_date;  
    
     [Column("vendor")]  
     public  long  Vendor  
     {  
         get { return _Vendor; }  
         set { _Vendor = value; Track("vendor"); }  
      }  
      long  _Vendor;  
    
 public static IEnumerable<Equipment_hub_Rent_Rate> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Rent_Rate (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Rent_Rate>(sql);
     }
  }


 [TableName("Equipment_hub_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_right_Admin : Equipment_hub.Record<Equipment_hub_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<Equipment_hub_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_right_Admin>(sql);
     }
  }


 [TableName("Equipment_hub_right_Vendor")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_right_Vendor : Equipment_hub.Record<Equipment_hub_right_Vendor>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rightname")]  
     public  string   Rightname  
     {  
         get { return _Rightname; }  
         set { _Rightname = value; Track("rightname"); }  
      }  
      string   _Rightname;  
    
 public static IEnumerable<Equipment_hub_right_Vendor> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_right_Vendor (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_right_Vendor>(sql);
     }
  }


 [TableName("Equipment_hub_role_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_role_Admin : Equipment_hub.Record<Equipment_hub_role_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
 public static IEnumerable<Equipment_hub_role_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_role_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_role_Admin>(sql);
     }
  }


 [TableName("Equipment_hub_role_right_Admin")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_role_right_Admin : Equipment_hub.Record<Equipment_hub_role_right_Admin>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<Equipment_hub_role_right_Admin> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_role_right_Admin (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_role_right_Admin>(sql);
     }
  }


 [TableName("Equipment_hub_role_right_Vendor")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_role_right_Vendor : Equipment_hub.Record<Equipment_hub_role_right_Vendor>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("role")]  
     public  long  Role  
     {  
         get { return _Role; }  
         set { _Role = value; Track("role"); }  
      }  
      long  _Role;  
    
     [Column("right")]  
     public  long  Right  
     {  
         get { return _Right; }  
         set { _Right = value; Track("right"); }  
      }  
      long  _Right;  
    
 public static IEnumerable<Equipment_hub_role_right_Vendor> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_role_right_Vendor (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_role_right_Vendor>(sql);
     }
  }


 [TableName("Equipment_hub_role_Vendor")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_role_Vendor : Equipment_hub.Record<Equipment_hub_role_Vendor>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("rolename")]  
     public  string   Rolename  
     {  
         get { return _Rolename; }  
         set { _Rolename = value; Track("rolename"); }  
      }  
      string   _Rolename;  
    
     [Column("company")]  
     public  long  Company  
     {  
         get { return _Company; }  
         set { _Company = value; Track("company"); }  
      }  
      long  _Company;  
    
 public static IEnumerable<Equipment_hub_role_Vendor> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_role_Vendor (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_role_Vendor>(sql);
     }
  }


 [TableName("Equipment_hub_Selected_Equipement")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Selected_Equipement : Equipment_hub.Record<Equipment_hub_Selected_Equipement>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("equipment_activity")]  
     public  long  Equipment_activity  
     {  
         get { return _Equipment_activity; }  
         set { _Equipment_activity = value; Track("equipment_activity"); }  
      }  
      long  _Equipment_activity;  
    
     [Column("equipment")]  
     public  long  Equipment  
     {  
         get { return _Equipment; }  
         set { _Equipment = value; Track("equipment"); }  
      }  
      long  _Equipment;  
    
 public static IEnumerable<Equipment_hub_Selected_Equipement> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Selected_Equipement (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Selected_Equipement>(sql);
     }
  }


 [TableName("Equipment_hub_Vendor_Company")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Vendor_Company : Equipment_hub.Record<Equipment_hub_Vendor_Company>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("company_name")]  
     public  string   Company_name  
     {  
         get { return _Company_name; }  
         set { _Company_name = value; Track("company_name"); }  
      }  
      string   _Company_name;  
    
 public static IEnumerable<Equipment_hub_Vendor_Company> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Vendor_Company (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Vendor_Company>(sql);
     }
  }


 [TableName("Equipment_hub_Vendor_Payments")]
 [PrimaryKey("id")]
 [ExplicitColumns]
 public partial class Equipment_hub_Vendor_Payments : Equipment_hub.Record<Equipment_hub_Vendor_Payments>
 {
     [Column("id")]  
     public long Id  
     {  
         get { return _Id; }  
         set { _Id = value; Track("id"); }  
      }  
     long _Id;  
    
     [Column("equipement_activity")]  
     public  long  Equipement_activity  
     {  
         get { return _Equipement_activity; }  
         set { _Equipement_activity = value; Track("equipement_activity"); }  
      }  
      long  _Equipement_activity;  
    
     [Column("amount")]  
     public   float   Amount  
     {  
         get { return _Amount; }  
         set { _Amount = value; Track("amount"); }  
      }  
       float   _Amount;  
    
     [Column("date_payment_was_due")]  
     public  DateTime  Date_payment_was_due  
     {  
         get { return _Date_payment_was_due; }  
         set { _Date_payment_was_due = value; Track("date_payment_was_due"); }  
      }  
      DateTime  _Date_payment_was_due;  
    
     [Column("date_payment_was_made")]  
     public  DateTime  Date_payment_was_made  
     {  
         get { return _Date_payment_was_made; }  
         set { _Date_payment_was_made = value; Track("date_payment_was_made"); }  
      }  
      DateTime  _Date_payment_was_made;  
    
 public static IEnumerable<Equipment_hub_Vendor_Payments> Query(Database db, string[] columns = null, long[] Id = null) {
  
     var sql = new Sql();
     if (columns != null)        sql.Select(columns);  
     sql.From("Equipment_hub_Vendor_Payments (NOLOCK)");  
       if (Id != null)
            sql.Where("id IN (@0)", Id);
  
       return db.Query<Equipment_hub_Vendor_Payments>(sql);
     }
  }

	}
