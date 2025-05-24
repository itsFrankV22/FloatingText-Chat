
# 🗨️ FloatingText-Chat

> [!NOTE]
> If you speak English visit [README_EN.md](./README_ENGLISH.md)

**TShock Plugin:** Muestra mensajes flotantes (burbujas de chat) sobre los jugadores cuando envían mensajes en el chat.

> Compatible con:  
> ✅ **[Floating-ItemDecoPlugin](https://github.com/itsFrankV22/ItemsDeco-Plugin)**

Ahora usamos Telemetria, al usar este y otros de mis plugins aceptas cualquier termino, ninguno de sus datos seran publicos

---

## 🚀 Características

- Muestra burbujas de texto encima del jugador al enviar un mensaje.
- Color del texto basado en el grupo del jugador.
- Sistema de permisos y filtros avanzados.
- Efectos de sonido personalizables.
- Comando `/quiet <message>` para enviar burbujas de texto silenciosas (sin mensaje de chat global).

---

## 🛠️ Instalación

1. **Descargar el Plugin**
   - Dirígete a la sección de [Releases](https://github.com/itsFrankV22/FloatingText-Chat/releases/).
   - Descarga el archivo `.zip` o `.dll` más reciente según tu versión de TShock.

2. **Instalar en tu servidor TShock**
   - Si descargaste un `.zip`, extrae el contenido.
   - Copia el archivo `.dll`.
   - Pega el `.dll` en la carpeta:  
     `TShock/ServerPlugins/`

3. **Reiniciar el servidor**
   - Reinicia tu servidor TShock para que el plugin se cargue.

4. **Verificar que está funcionando**
   - Revisa la consola del servidor o usa `/plugins` dentro del juego para confirmar que se cargó correctamente.

---

## ⚙️ Ejemplo de configuración (`config.json`)

Este archivo se genera automáticamente en `TShock/FloatingText/config.json`. Puedes editarlo para personalizar el comportamiento del plugin:

```jsonc
{
  "General": {
    "EnableInitializationRequest": true,
    "ExcludedGroups": [ "Guest" ],
    "RequireRegistration": true
  },
  "Filters": {
    "PlayerNotDead": true,
    "RequirePermission": false,
    "Permission": "floatingtext.show"
  },
  "Sound": {
    "Volume": 0.6
  }
}
```

---

## 🧪 Comando incluido

| Comando | Descripción | Permiso necesario |
|--------|-------------|-------------------|
| `/quiet` | Envía una burbuja de texto sin mostrar el mensaje en el chat global. | `floatingtext.quiet` |

---

## 🧠 Notas técnicas

- Utiliza `NetMessage.SendData(119, ...)` para mostrar los textos flotantes.
- El color del mensaje se basa en el color del grupo (`Group.R, G, B`).
- Soporte completo para reproducir sonidos usando `NetMessage.PlayNetSound(...)`, con volumen y pitch personalizables.
- El plugin implementa filtros como jugador muerto, permisos o grupos excluidos.

---

## 🧰 Requisitos

- TShock 4.5.0+  
- Terraria Server 1.4.4.9+  
- Compatible con SQLite o MySQL si decides extender la funcionalidad.

---

## 🧑‍💻 Créditos

Desarrollado por [FrankV22](https://github.com/itsFrankV22)  
Inspirado en la estética de juegos modernos con burbujas de chat animadas.

---

¿Quieres contribuir? ¡Pull requests son bienvenidos!
