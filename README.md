# 🃏 SBGAO Server - 联机卡牌游戏服务端

本项目是一个基于 C# 开发的独立联机卡牌游戏服务端。它为 Unity 客户端提供支持，采用 WebSocket 进行实时通信。项目核心架构专注于 **高可扩展性** 和 **完全的服务器主权 (Authoritative Server)**，确保游戏逻辑的严谨性和防作弊。

## 核心特性

*    **状态主权 (Authoritative Server)**: 卡牌的抽取、打出、结算逻辑**完全由服务器执行**。客户端仅作为“显示器”和“输入设备”，从根本上杜绝修改内存等作弊手段。
*    **逻辑与表现彻底隔离**: 服务器端没有任何 Unity 的 `GameObject` 或动画逻辑，纯粹进行数据推演。
*    **核心指令系统 (Action System)**: 采用动作队列机制（Action Queue）来处理复杂的卡牌结算、连锁反应和被动触发，确保游戏状态的绝对同步和顺序执行。
*    **轻量级网络通信**: 底层采用 `Fleck` 提供 WebSocket 服务，使用 `Newtonsoft.Json` 进行高效的数据序列化与反序列化。

## 架构演进与致谢 (Credits)

本项目的核心系统（**Action System**）的灵感与架构原型，深度参考了 YouTube 博主[@thecodeotter](https://www.youtube.com/@thecodeotter) 的[《Slay The Spire (杀戮尖塔)》仿制教程](https://youtu.be/rgsp9pb0Oi0?si=HbODrMei-AJWz_Gs)。

*   **从单机到联机的重构**：原教程为基于 Unity 的单机游戏架构。本项目在此基础上进行了深度改造，将核心的 `Action` 结算逻辑完全抽离到 C# 独立服务器中。
*   **指令驱动机制**：服务器执行 Action 后，不会直接播放动画，而是生成对应的状态变化数据（State Delta）并封装为 JSON 发送给 Unity 客户端，客户端据此将数据映射为表现层的动画（Animation）。

## 🛠️ 技术栈

*   **语言**: C# / .NET
*   **网络库**: [Fleck](https://github.com/statianzo/Fleck) (WebSocket)
*   **JSON 解析**: [Newtonsoft.Json](https://www.newtonsoft.com/json)
