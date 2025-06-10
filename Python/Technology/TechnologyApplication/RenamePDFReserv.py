import fitz  # PyMuPDF
import os
import re

def normalize_dashes_and_spaces(text):
    text = re.sub(r'[\u00AD\u2010\u2011\u2012\u2013\u2014\u2015]', '-', text)
    return text.strip()

def clean_filename(name):
    name = re.sub(r'\s+', ' ', name).strip()
    name = re.sub(r'[\\/*?:"<>|]', '', name)
    name = name.replace("Сборочный чертеж", "").replace("Чертеж", "").replace("Сборочный", "")
    return re.sub(r'\s+', ' ', name).strip()

def merge_pdfs(main_path, extra_path):
    """Добавляет страницы из extra в основной PDF"""
    try:
        doc_main = fitz.open(main_path)
        doc_extra = fitz.open(extra_path)

        doc_main.insert_pdf(doc_extra)  # Добавляем все страницы из extra в конец main
        doc_main.saveIncr()  # Сохраняем инкрементально — без полного пересохранения
        doc_main.close()
        doc_extra.close()

        print(f"[OK] '{os.path.basename(extra_path)}' добавлен в '{os.path.basename(main_path)}'")
        os.remove(extra_path)
        return True
    except Exception as e:
        print(f"[Ошибка при слиянии]: {e}")
        return False

folder_path = r'C:\PDF\1\Rename'

# Два набора регионов: для разных форматов листа
rects_A = [
    fitz.Rect(830, 676, 1161, 709),   # Регион A1 — извлечение номера документа
    fitz.Rect(830, 720, 1019, 750)    #fitz.Rect(830, 720, 1019, 780)     # Регион A2 – наименование детали
]

rects_B = [
    fitz.Rect(245, 676, 576, 709),   # Регион B1 – номер документа
    fitz.Rect(245, 720, 434, 750)    #fitz.Rect(245, 720, 434, 780)    # Регион B2 – наименование детали
]

for filename in os.listdir(folder_path):
    if filename.lower().endswith(".pdf"):
        file_path = os.path.join(folder_path, filename)

        # Шаг 1: Проверка на "-001" и слияние с основным файлом
        if re.search(r'-001\.pdf$', filename, re.IGNORECASE):
            base_name = re.sub(r'-001\.pdf$', '.pdf', filename, flags=re.IGNORECASE)
            base_path = os.path.join(folder_path, base_name)

            if os.path.exists(base_path):
                print(f"[INFO] Найден базовый файл для слияния: {base_name}")

                try:
                    doc_base = fitz.open(base_path)
                    doc_extra = fitz.open(file_path)

                    doc_base.insert_pdf(doc_extra)
                    doc_base.saveIncr()  # Сохранение без пересоздания всего файла
                    doc_base.close()
                    doc_extra.close()

                    os.remove(file_path)
                    print(f"[MERGED] Страницы из '{filename}' добавлены в '{base_name}'")
                except Exception as e:
                    print(f"[Ошибка при сохранении] {e}")
                    continue
            else:
                print(f"[!] Базовый файл '{base_name}' не найден → пропуск")

        # Шаг 2: Пропускаем файлы с "-001", т.к. они уже обработаны
        if re.search(r'-001\.pdf$', filename, re.IGNORECASE):
            continue

        try:
            doc = fitz.open(file_path)
            page = doc.load_page(0)

            # Определяем формат листа
            page_size = page.rect
            width, height = page_size.width, page_size.height
            is_large_sheet = width > 800 or height > 1000  # Теперь так, как ты просил

            regions = rects_A if is_large_sheet else rects_B

            lines = []
            for rect in regions:
                clip_text = page.get_text("text", clip=rect).strip()
                region_lines = [line.strip() for line in clip_text.splitlines() if line.strip()]
                lines.extend(region_lines[:2])  # берем по 2 строки из каждого региона

            # Извлечение данных
            if len(lines) >= 2:
                number_line = lines[0]
                name_line = lines[1]
            else:
                print(f"[!] Недостаточно данных в '{filename}'")
                continue

            # Чистка имени
            number_line = normalize_dashes_and_spaces(number_line)
            name_line = clean_filename(name_line)

            new_name = f"{number_line} {name_line}.pdf"
            new_file_path = os.path.join(folder_path, new_name)

            # Закрываем документ до переименования
            doc.close()

            # Переименование
            if not os.path.exists(new_file_path):
                os.rename(file_path, new_file_path)
                print(f"[RENAMED] '{filename}' → '{new_name}'")
            else:
                print(f"[!] Файл '{new_file_path}' уже существует.")

        except Exception as e:
            print(f"[Ошибка] {filename}: {e}")

input("\nНажмите Enter для выхода...")