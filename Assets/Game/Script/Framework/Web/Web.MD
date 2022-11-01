文件夹路径 Assets\Game\Script\Framework\Web

```
IServerSettings 接口 
   ServerSettings相关 
   服务url配置相关

```

git地址---> [NetworkManager](https://github.com/myzjy/MmoGameDemoUnity/blob/273a866341062d643abb64f0fd60713d86b7d9ae/Assets/Game/Script/Framework/Web/NetworkManager.cs)

本地路径--->[NetworkManager](NetworkManager.cs)

```
NetworkManager
  继承[SpringComponent]
  
  OnAwake 方法
  打开时调用
初始化方法Init
  public void Init()
  {
      Debug.Log("初始化NetworkManager");
      this.UserAuth = new UserAuth();
      this._handler = gameObject.AddComponent<ApiHandler>();
      this._handler.Setup(SpringContext.GetBean<ISettingManager>().GetHttpsBase(),
          settingManager.GetString(GameConstant.SETTING_LOGIN_TOKEN));
      this._handler.onResponseGlobal += SetAuthTokenOnce;
      this._handler.onCompleteGlobal += RequestCompleteGlobal;
      this._handler.onErrorGlobal += ErrorGlobal;
  }
```