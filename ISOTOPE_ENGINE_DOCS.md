ISOTOPE ENGINE: Technical Design Document

Версия: 1.0
Статус: Draft
Язык: C# (.NET 9)
Архитектура: Data-Oriented (ECS)

1. Философия и Цели

ISOTOPE — это высокопроизводительный 2D игровой движок, предназначенный для создания сложных системных симуляторов (Immersive Sims) с видом сверху (Top-Down) и поддержкой мультиплеера.

Ключевые принципы:

Data-Driven: Движок не содержит игрового контента (нет класса Gun). Он предоставляет системы для обработки данных. Контент загружается из внешних определений (Code/JSON).

Performance First: Использование structs, Span<T> и ECS (Arch) для минимизации аллокаций памяти (GC Pressure).

Server Authoritative: Логика выполняется на сервере. Клиент — это "глупый терминал" для визуализации и отправки ввода.

Simulation Core: Встроенная поддержка клеточных автоматов (газы, жидкости, огонь) на уровне ядра.

2. Стек Технологий

Core Language: C# 12 / .NET 9.

ECS Framework: Arch (High-performance Archetype ECS).

Rendering: Raylib-cs (Bindings for Raylib).

Networking: LiteNetLib (Reliable UDP).

Serialization: MessagePack или MemoryPack (Binary zero-copy).

Math: System.Numerics.Vectors.

3. Архитектура Решения (Solution Structure)

Проект делится на 3 уровня абстракции:

A. Isotope.Core (Shared Library)

Содержит общие типы данных для Клиента и Сервера.

Components: Структуры данных (Position, Velocity, SpriteId).

Grid System: Логика хранения карты, чанки, TileDefinition.

Systems Base: Базовые классы для ECS систем.

NetProtocol: Пакеты данных (Packets) и методы сериализации.

B. Isotope.Engine (Server / Logic Host)

"Мозг" движка. Работает в headless режиме (без графики).

GameLoop: Фиксированный шаг времени (TickRate: 60hz).

Simulation Systems: Атомосфера, Физика столкновений, Инвентарь.

State Replication: Система, собирающая изменения (Dirty Components) и отправляющая их клиентам.

C. Isotope.Client (Visualizer)

"Глаза" движка.

Renderer: Отрисовка тайлов, спрайтов, света (Lighting), UI.

Input Handler: Сбор нажатий и отправка их на сервер.

Interpolation: Плавное сглаживание движений между тиками сервера.

Prediction: Локальное предсказание движения (Client-side prediction) для отзывчивости.

4. Подсистема: Grid & Map (Сетка и Карта)

Движок использует Чанковую систему для поддержки больших миров.

4.1. Реестр (Registry)

Все объекты в мире имеют ID. Движок не знает, что такое "Бетонная стена", он знает TileID: 5.

code
C#
download
content_copy
expand_less
public class TileDefinition {
    public ushort Id;
    public string InternalName; // "wall_concrete"
    public string TexturePath;
    public bool IsSolid;
    public float ThermalConductivity; // Для атмосферы
}
4.2. Хранение данных (The Grid)

Карта состоит из Чанков (Chunks) размером 16x16 или 32x32.

Используется Dictionary<(int, int), Chunk> для бесконечной карты.

Или Chunk[] для карт фиксированного размера (быстрее).

Каждый тайл хранит Слои (Layers):

Floor Layer: (Пол, космос).

Object Layer: (Стены, окна, шлюзы).

Pipe/Wire Layer: (Провода, трубы — невидимы или полупрозрачны).

5. Подсистема: ECS (Сущности)

Все динамические объекты (Игроки, Предметы, Пули) — это Entity в Arch.

5.1. Основные Компоненты (Built-in Components)

Движок поставляет базовый набор:

TransformComponent: Координаты (World Vector2).

PhysicsBodyComponent: Размер (AABB), Вес, Трение.

SpriteComponent: Какой спрайт рисовать, цвет, слой (Z-index).

NetworkIdentity: Уникальный ID для синхронизации по сети.

InputComponent: (Только на сервере) Текущие намерения игрока (MoveDir, Action).

5.2. Жизненный цикл (Game Loop)

Network Receive: Получение пакетов ввода.

Pre-Physics: Применение сил, скоростей.

Physics Step: Проверка коллизий AABB vs Grid (Стены).

Logic Step: Скрипты взаимодействия, таймеры.

Atmos Step: (Раз в N тиков) Диффузия газов.

Replication: Сбор изменений -> Отправка клиентам.

6. Подсистема: Ресурсы и Моддинг

Движок должен поддерживать Hot Reloading контента.

6.1. Resource Manager

При старте сканирует папку /Resources.

Загружает tiles.json, items.json.

Автоматически собирает Атлас текстур (Texture Atlas) из отдельных спрайтов (для оптимизации DrawCalls).

6.2. Прототипы (Prototypes)

Вместо хардкода, сущности создаются из шаблонов.
Example (YAML/JSON):

code
Yaml
download
content_copy
expand_less
- type: entity
  id: human
  components:
    - type: Sprite
      texture: "mobs/human.png"
    - type: Physics
      mass: 70
    - type: Health
      max: 100

Движок парсит этот файл и создает Entity с нужным набором компонентов.

7. Сетевой протокол (Networking)
7.1. Snapshot Interpolation

Сервер шлет состояние мира (Snapshot) 20 раз в секунду.
Клиент хранит буфер из 2-3 последних снимков и интерполирует (смешивает) позиции между ними. Это дает плавность при 60/144 FPS, даже если сеть лагает.

7.2. Delta Compression

Чтобы экономить трафик, сервер не шлет всю карту. Он шлет:

Только видимые игроку чанки.

Только изменившиеся сущности (Delta).

8. Дорожная карта (Roadmap)
Phase 1: The Skeleton (Текущая цель)

Поднять проект (Core, Client, Server).

Реализовать TileRegistry и рендер статической карты из массива.

Реализовать Camera2D с зумом и перемещением.

Phase 2: The Mover

Подключить Arch ECS.

Создать сущность "Игрок".

Реализовать физику движения (AABB vs TileMap).

Phase 3: The Network

Подключить LiteNetLib.

Разделить ввод (Клиент) и симуляцию (Сервер).

Реализовать репликацию позиций.

Phase 4: The Interaction

Система инвентаря (Container Component).

Система взаимодействия (Click -> Use).

UI на ImGui.