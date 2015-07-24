using System;
using System.Collections.Generic;
using Android.App; 
using Android.Widget;
using Android.OS;
using Android.Graphics; 
using System.IO;

namespace RoundedCornerImage
{
	[Activity ( Label = "CirclularImageBitMap" , MainLauncher = true , Icon = "@drawable/icon" )]
	public class MainActivity : Activity
	{ 
		 string strImgPath;
		 protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );  
			Window.RequestFeature ( Android.Views.WindowFeatures.NoTitle );
			SetContentView ( Resource.Layout.Main );
			ImageView imgProfilePic = FindViewById<ImageView> ( Resource.Id.myImg );
			BindListView ();  //make listviewvisibility :visible
			imgProfilePic.Click += async delegate(object sender , EventArgs e )
			{  
				strImgPath = System.IO.Path.Combine ( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath , "Sample" );  
				strImgPath = System.IO.Path.Combine ( strImgPath , "sampleOriginal.jpg" ); 

				int width = imgProfilePic.MeasuredWidth / 2;
				int height = imgProfilePic.MeasuredHeight / 2;

				BitmapFactory.Options option = await ImageHelper.GetBitmapOptionsOfImage ( strImgPath );
			 
				var bitmapSampled = await ImageHelper.LoadScaledDownBitmapForDisplayAsync ( strImgPath , option , width , height );  

				//to get cirlcle shape H*W should be same 
				int intRoundPicResolution; 
				if ( bitmapSampled.Height > bitmapSampled.Width )  //only to get circle shape
					intRoundPicResolution = bitmapSampled.Width;
				else
					intRoundPicResolution = bitmapSampled.Height;
				//set bitmap height and width 
				bitmapSampled = Bitmap.CreateScaledBitmap ( bitmapSampled , intRoundPicResolution , intRoundPicResolution , false );

				int imgRadius = ( bitmapSampled.Width ) / 2;  
				using ( var bitmapRoundedCorner = ImageHelper.GetRoundedCornerBitmap ( bitmapSampled , imgRadius ) )
				{
					ImageHelper.RecycleBitmap ( imgProfilePic );
					imgProfilePic.SetImageBitmap ( bitmapRoundedCorner ); 
				} 

				try
				{ 
					string strNewImgPath = System.IO.Path.Combine ( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath , "Sample" , "sampleOptimized.jpg" );  
					var fileStream = new FileStream ( strNewImgPath , FileMode.Create );
					bitmapSampled.Compress ( Bitmap.CompressFormat.Png , 80 , fileStream );//writing file in local with scaled down bitmap
					fileStream.Close (); 
					bitmapSampled.Dispose ();
					bitmapSampled = null;  
				}
				catch ( Exception es )
				{ 
					Console.WriteLine ( es.Message );
				}
			};
		}
 
		List<PersonClass> lstPerson;
		void BindListView() 
		{ 
			lstPerson = new List<PersonClass> ();
			RoundedCornerAdapterClass objPersonAdapter;
			var lstViewPerson = FindViewById<ListView> ( Resource.Id.listViewPerson );
			FillList ();
			if(lstPerson!=null && lstPerson.Count >0)
			{ 
				objPersonAdapter = new RoundedCornerAdapterClass (this, lstPerson); 
				lstViewPerson.Adapter = objPersonAdapter; 
			}
		}
		void FillList()
		{
			PersonClass objPerson; 
			string strProfilePath = System.IO.Path.Combine ( Android.OS.Environment.ExternalStorageDirectory.AbsolutePath , "SHAREit" ,"pictures");
			for (int i = 0; i <12 ; i++)
			{ 
				objPerson = new PersonClass ();
				objPerson.PersonName =string.Format( "ProfilePic_{0} ",i);
				objPerson.PersonImage = System.IO.Path.Combine(strProfilePath,string.Format("{0}.jpg",i));
				lstPerson.Add (objPerson); 
			} 
		}



	}
}


