using System;
using System.Collections.Generic;

namespace Microsoft.Arcadia.Marketplace.PackageObjectModel.Permission
{
	public static class PermissionMap
	{
		private static IReadOnlyDictionary<string, PermissionMapItem> allPermissions = new Dictionary<string, PermissionMapItem>
	{
		{
			"android.permission.ACCESS_COARSE_LOCATION",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("location", CapabilityType.Device)
			})
		},
		{
			"android.permission.ACCESS_CHECKIN_PROPERTIES",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.ACCESS_FINE_LOCATION",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("location", CapabilityType.Device)
			})
		},
		{
			"android.permission.ACCESS_LOCATION_EXTRA_COMMANDS",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("location", CapabilityType.Device)
			})
		},
		{
			"android.permission.ACCESS_MOCK_LOCATION",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.ACCESS_NETWORK_STATE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software)
			})
		},
		{
			"android.permission.ACCESS_SURFACE_FLINGER",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.ACCESS_WIFI_STATE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software)
			})
		},
		{
			"android.permission.BIND_ACCESSIBILITY_SERVICE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.ACCOUNT_MANAGER",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.AUTHENTICATE_ACCOUNTS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BATTERY_STATS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BIND_APPWIDGET",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BIND_DEVICE_ADMIN",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_INPUT_METHOD",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_NFC_SERVICE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_PRINT_SERVICE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_REMOTEVIEWS",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_TEXT_SERVICE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_VPN_SERVICE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BIND_WALLPAPER",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.BLUETOOTH",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("bluetooth", CapabilityType.Device)
			})
		},
		{
			"android.permission.BLUETOOTH_ADMIN",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("radios", CapabilityType.Device)
			})
		},
		{
			"android.permission.BLUETOOTH_PRIVILEGED",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BRICK",
			new PermissionMapItem(PermissionType.Radioactive, null)
		},
		{
			"android.permission.BROADCAST_SMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BROADCAST_STICKY",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.BROADCAST_WAP_PUSH",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.CAMERA",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("webcam", CapabilityType.Device)
			})
		},
		{
			"android.permission.CALL_PHONE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.CALL_PRIVILEGED",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.CAPTURE_AUDIO_OUTPUT",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("microphone", CapabilityType.Device)
			})
		},
		{
			"android.permission.CAPTURE_SECURE_VIDEO_OUTPUT",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CAPTURE_VIDEO_OUTPUT",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CHANGE_COMPONENT_ENABLED_STATE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CHANGE_CONFIGURATION",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CHANGE_NETWORK_STATE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software)
			})
		},
		{
			"android.permission.CHANGE_WIFI_MULTICAST_STATE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software)
			})
		},
		{
			"android.permission.CHANGE_WIFI_STATE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software)
			})
		},
		{
			"android.permission.CLEAR_APP_CACHE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CLEAR_APP_USER_DATA",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.CONTROL_LOCATION_UPDATES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.DELETE_CACHE_FILES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.DELETE_PACKAGES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.DEVICE_POWER",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.DIAGNOSTIC",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.DISABLE_KEYGUARD",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.DUMP",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.EXPAND_STATUS_BAR",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.FACTORY_TEST",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.FLASHLIGHT",
			new PermissionMapItem(PermissionType.Present, null)
		},
		{
			"android.permission.FORCE_BACK",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.GET_ACCOUNTS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.GET_PACKAGE_SIZE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.GET_TASKS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.GET_TOP_ACTIVITY_INFO",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.GLOBAL_SEARCH",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.HARDWARE_TEST",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.INJECT_EVENTS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.INSTALL_LOCATION_PROVIDER",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.INSTALL_PACKAGES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.INTERNAL_SYSTEM_WINDOW",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.INTERNET",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("internetClient", CapabilityType.Software),
				new AppxCapability("internetClientServer", CapabilityType.Software),
				new AppxCapability("privateNetworkClientServer", CapabilityType.Software)
			})
		},
		{
			"android.permission.LOCATION_HARDWARE",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.MANAGE_ACCOUNTS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.MANAGE_APP_TOKENS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.MANAGE_DOCUMENTS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.MASTER_CLEAR",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.MEDIA_CONTENT_CONTROL",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.MODIFY_AUDIO_SETTINGS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.MODIFY_PHONE_STATE",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.MOUNT_FORMAT_FILESYSTEMS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.MOUNT_UNMOUNT_FILESYSTEMS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.NFC",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("proximity", CapabilityType.Device)
			})
		},
		{
			"android.permission.PERSISTENT_ACTIVITY",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.PROCESS_OUTGOING_CALLS",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.READ_CALENDAR",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("appointments", CapabilityType.Software)
			})
		},
		{
			"android.permission.READ_CONTACTS",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("contacts", CapabilityType.Software)
			})
		},
		{
			"android.permission.READ_EXTERNAL_STORAGE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("removableStorage", CapabilityType.Software)
			})
		},
		{
			"android.permission.READ_FRAME_BUFFER",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_INPUT_STATE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_LOGS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_PHONE_STATE",
			new PermissionMapItem(PermissionType.Present, null)
		},
		{
			"android.permission.READ_PROFILE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_SMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_SOCIAL_STREAM",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_SYNC_SETTINGS",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("contacts", CapabilityType.Software)
			})
		},
		{
			"android.permission.READ_SYNC_STATS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.READ_USER_DICTIONARY",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.REBOOT",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.RECEIVE_BOOT_COMPLETED",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.RECEIVE_MMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.RECEIVE_SMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.RECEIVE_WAP_PUSH",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.RECORD_AUDIO",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("microphone", CapabilityType.Device)
			})
		},
		{
			"android.permission.REORDER_TASKS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.RESTART_PACKAGES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SEND_SMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.SEND_RESPOND_VIA_MESSAGE",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_ACTIVITY_WATCHER",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.SET_ALWAYS_FINISH",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_ANIMATION_SCALE",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_DEBUG_APP",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_ORIENTATION",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_POINTER_SPEED",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_PREFERRED_APPLICATIONS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.SET_PROCESS_LIMIT",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_TIME",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SET_TIME_ZONE",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.SET_WALLPAPER",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.SET_WALLPAPER_HINTS",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.SIGNAL_PERSISTENT_PROCESSES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.STATUS_BAR",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.SUBSCRIBED_FEEDS_READ",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.SUBSCRIBED_FEEDS_WRITE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.SYSTEM_ALERT_WINDOW",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.TRANSMIT_IR",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.VIBRATE",
			new PermissionMapItem(PermissionType.Present, null)
		},
		{
			"android.permission.WRITE_CONTACTS",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("contacts", CapabilityType.Software)
			})
		},
		{
			"android.permission.WRITE_EXTERNAL_STORAGE",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("removableStorage", CapabilityType.Software)
			})
		},
		{
			"android.permission.WRITE_SECURE_SETTINGS",
			new PermissionMapItem(PermissionType.Radioactive, null)
		},
		{
			"android.permission.WRITE_SOCIAL_STREAM",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("contacts", CapabilityType.Software)
			})
		},
		{
			"android.permission.WRITE_SYNC_SETTINGS",
			new PermissionMapItem(PermissionType.Present, new List<AppxCapability>
			{
				new AppxCapability("contacts", CapabilityType.Software)
			})
		},
		{
			"android.permission.UPDATE_DEVICE_STATS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.USE_CREDENTIALS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.USE_SIP",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.WAKE_LOCK",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.WRITE_APN_SETTINGS",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.WRITE_CALENDAR",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.WRITE_CALL_LOG",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"android.permission.WRITE_GSERVICES",
			new PermissionMapItem(PermissionType.ThirdPartyMissing, null)
		},
		{
			"android.permission.WRITE_PROFILE",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.WRITE_SETTINGS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.WRITE_SMS",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"android.permission.WRITE_USER_DICTIONARY",
			new PermissionMapItem(PermissionType.RuntimeUnknown, null)
		},
		{
			"C2D_MESSAGE",
			new PermissionMapItem(PermissionType.GmsMessaging, null)
		},
		{
			"c2dm.permission.RECEIVE",
			new PermissionMapItem(PermissionType.GmsMessaging, null)
		},
		{
			"c2dm.permission.REGISTRATION",
			new PermissionMapItem(PermissionType.GmsMessaging, null)
		},
		{
			"c2dm.permission.SEND",
			new PermissionMapItem(PermissionType.GmsMessaging, null)
		},
		{
			"com.amazon.inapp.purchasing.Permission.NOTIFY",
			new PermissionMapItem(PermissionType.AmazonPurchase, null)
		},
		{
			"com.amazon.device.messaging",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.device.messaging.intent.RECEIVE",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.device.messaging.intent.REGISTRATION",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.device.messaging.permission.RECEIVE",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.device.messaging.permission.SEND",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.inapp.purchasing.NOTIFY",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.inapp.purchasing.ResponseReceiver",
			new PermissionMapItem(PermissionType.AmazonPush, null)
		},
		{
			"com.amazon.geo.maps",
			new PermissionMapItem(PermissionType.AmazonMap, null)
		},
		{
			"com.android.alarm.permission.SET_ALARM",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"com.android.browser.permission.READ_HISTORY_BOOKMARKS",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"com.android.browser.permission.WRITE_HISTORY_BOOKMARKS",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"com.android.launcher.permission.INSTALL_SHORTCUT",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"com.android.launcher.permission.UNINSTALL_SHORTCUT",
			new PermissionMapItem(PermissionType.Missing, null)
		},
		{
			"com.android.vending.BILLING",
			new PermissionMapItem(PermissionType.GmsBilling, null)
		},
		{
			"com.android.vending.CHECK_LICENSE",
			new PermissionMapItem(PermissionType.GmsLicensing, null)
		},
		{
			"com.android.voicemail.permission.ADD_VOICEMAIL",
			new PermissionMapItem(PermissionType.Missing, null)
		}
	};

		public static PermissionMapItem MapPermission(string androidPermission)
		{
			if (androidPermission == null)
			{
				throw new ArgumentNullException("androidPermission");
			}
			if (allPermissions.ContainsKey(androidPermission))
			{
				return allPermissions[androidPermission];
			}
			return new PermissionMapItem(PermissionType.RuntimeUnknown, null);
		}
	}
}