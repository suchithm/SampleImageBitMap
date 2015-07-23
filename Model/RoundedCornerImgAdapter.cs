using System;
using Android.Views;
using Android.Widget;
using Android.App;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Threading.Tasks;

  
namespace RoundedCornerImage
{
	public class RoundedCornerAdapterClass :BaseAdapter
	{
		Activity _context;
		List<PersonClass> _lstItem;  
		ViewHolderItem viewHolder;   
		public RoundedCornerAdapterClass (Activity c,List<PersonClass> lstIem)
		{
			_context = c;
			_lstItem = lstIem;  
		}
		public override int Count {
			get { return _lstItem.Count; }
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return null;
		}
		public override long GetItemId (int position)
		{
			return 0;
		} 
		public  override   View GetView (int position,  View convertView,  ViewGroup parent)
		{ 
			View rowView = convertView;
			//reuse view
			if (rowView == null) { 
				rowView = _context.LayoutInflater.Inflate (Resource.Layout.PersonCustomLayout, parent, false); 
				viewHolder = new ViewHolderItem ();
				viewHolder.txtTemName = rowView.FindViewById<TextView> (Resource.Id.lblPersonName);
				viewHolder.imgItem = rowView.FindViewById<ImageView> (Resource.Id.imgPerson);  
				rowView.Tag = viewHolder;
			} 
			else
			{
				viewHolder = (ViewHolderItem)rowView.Tag; 
			}  
			viewHolder.txtTemName.Text = _lstItem [position].PersonName;  

			var option=  ImageHelper.GetBitmapOptionsOfImageNonAsync ( _lstItem[position].PersonImage );

			var bitmapSampled= ImageHelper.LoadScaledDownBitmapForDisplay ( _lstItem[position].PersonImage , option , 100 , 100 );  
		
			//to get cirlcle shape H*W should be same 
			int intRoundPicResolution; 
			if ( bitmapSampled.Height > bitmapSampled.Width )  //only to get circle shape
				intRoundPicResolution = bitmapSampled.Width;
			else
				intRoundPicResolution = bitmapSampled.Height;
			
			//set bitmap height and width 
			bitmapSampled = Bitmap.CreateScaledBitmap ( bitmapSampled , intRoundPicResolution , intRoundPicResolution , false );
			int imgRadius = ( intRoundPicResolution ) / 2;  
			using ( var bitmapRoundedCorner =ImageHelper.GetRoundedCornerBitmap ( bitmapSampled , imgRadius ) )
			{
//				if (viewHolder.imgItem != null && !viewHolder.imgItem.isrecycled))
//				viewHolder.imgItem.RecycleBitmap ();
				viewHolder.imgItem.SetImageBitmap ( bitmapRoundedCorner ); 
			}  
			bitmapSampled.Dispose ();
			bitmapSampled = null;  
			Java.Lang.JavaSystem.Gc ();
			return  rowView;
		}
 
		class ViewHolderItem :Java.Lang.Object
		{
			internal   TextView txtTemName;
			internal   ImageView imgItem;    
		} 
	}
}
