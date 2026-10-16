import React, { useEffect } from 'react';
import './ConfirmModal.css';

function ConfirmModal({ isOpen, onClose, onConfirm, title, message, confirmText = 'Confirmar', cancelText = 'Cancelar', type = 'danger', showCancel = true }) {
  useEffect(() => {
    if (isOpen) {
      // Previne scroll do body quando modal está aberto
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = '';
    }

    return () => {
      document.body.style.overflow = '';
    };
  }, [isOpen]);

  useEffect(() => {
    const handleEscape = (e) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener('keydown', handleEscape);
    }

    return () => {
      document.removeEventListener('keydown', handleEscape);
    };
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const handleBackdropClick = (e) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  // Extrai ícone do título se presente
  const getIconFromTitle = (titleText) => {
    // Detecta emojis ou símbolos no início do título
    const iconMatch = titleText.match(/^([⚠️✅❌📂🗑️]|⚠|✅|❌|📂|🗑️)/);
    if (iconMatch) {
      return {
        icon: iconMatch[0],
        text: titleText.replace(iconMatch[0], '').trim()
      };
    }
    return { icon: null, text: titleText };
  };

  const { icon, text } = getIconFromTitle(title);
  const getIconType = () => {
    if (icon === '⚠️' || icon === '⚠') return 'warning';
    if (icon === '❌') return 'danger';
    if (icon === '✅') return 'success';
    if (icon === '📂') return 'info';
    return type;
  };

  return (
    <div className="modal-overlay" onClick={handleBackdropClick}>
      <div className={`modal-container modal-${type}`}>
        <div className="modal-header">
          <div className="modal-title-wrapper">
            {icon && (
              <span className={`modal-icon modal-icon-${getIconType()}`}>
                {icon}
              </span>
            )}
            <h3 className="modal-title">{text || title}</h3>
          </div>
          <button className="modal-close-btn" onClick={onClose} aria-label="Fechar">
            ✕
          </button>
        </div>
        <div className="modal-body">
          <p className="modal-message">{message}</p>
        </div>
        <div className="modal-footer">
          {showCancel && (
            <button className="modal-btn modal-btn-cancel" onClick={onClose}>
              {cancelText}
            </button>
          )}
          <button className={`modal-btn modal-btn-confirm modal-btn-${type}`} onClick={onConfirm}>
            {confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}

export default ConfirmModal;

