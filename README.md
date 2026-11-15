# 吉星派对 · 解锁全角色

> 《吉星派对》（Steam：AstralParty）的 CesiumLoader 客户端插件：让角色列表显示所有角色为已拥有，不需要改存档。

提示：请勿在使用本插件的情况下进行抽卡，后果自负！！！
解锁未解锁的角色，但PVE等级固定为0级。推荐在PVP等无等级的模式中使用，请低调使用。

---

## 特点

| 特点 | 说明 |
|---|---|
| **客户端解锁** | 只改本机 `HeroCardData` 的拥有标记，不写账号数据 |
| **不改存档** | 不碰服务器、不伪造 `RoleCard`，退出即恢复 |
| **PVE 等级 0** | 新解锁角色没有 PVE 等级，适合 PVP 等无等级模式 |
| **PC / Android 通用** | 同一个 `netstandard2.0` DLL，不需要分平台 |
| **删除即卸载** | 删掉 mod 目录就完全恢复 |

---

## 安装

1. 完全退出游戏；
2. 把文件放进：

```
mods/UnlockAllCharacters/
├─ UnlockAllCharacters.dll
├─ UnlockAllCharacters.json
└─ icon.png
```

3. 启动游戏，打开「角色」页面。

---

## 验证

角色页面右下角应显示：

```
已拥有角色: 34/34
```

日志里会出现：

```
[全角色解锁] 全角色解锁: 38 个角色（本次修改 32）
```

不同版本角色总数可能不同。

---

## 原理

1. 找到 `GameLogicManager.heroCard`；
2. 反射拿到私有字段 `heroCardDict`；
3. 把每个 `HeroCardData._IsHas` 设为 `true`；
4. `HeroCardData.IsHas` / `heroStatus` 都读这个字段，所以角色列表、详情、选人界面显示为已解锁。

只影响客户端 UI，服务器仍按账号真实角色校验。

---

## 注意

- **不要在使用本插件时抽卡**，后果自负；
- 联机选人可能仍受服务器校验限制；
- 新解锁角色的 PVE 等级为 0；
- 请低调使用。

---

## 构建

```
dotnet build src/Client/UnlockAllCharacters.csproj -c Release
```

输出：

```
src/Client/bin/Release/netstandard2.0/UnlockAllCharacters.dll
```

发布文件：

```
dist/UnlockAllCharacters.dll
dist/UnlockAllCharacters.json
dist/icon.png
```
